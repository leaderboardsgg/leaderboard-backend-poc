using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions.Specialized;
using LeaderboardBackend.Models;
using LeaderboardBackend.Models.Entities;
using LeaderboardBackend.Models.Requests;
using LeaderboardBackend.Models.ViewModels;
using LeaderboardBackend.Services;
using LeaderboardBackend.Test.Lib;
using LeaderboardBackend.Test.TestApi;
using LeaderboardBackend.Test.TestApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using NUnit.Framework;

namespace LeaderboardBackend.Test;

[TestFixture]
internal class Categories
{
    private static TestApiClient _apiClient = null!;
    private static WebApplicationFactory<Program> _factory = null!;
    private static readonly FakeClock _clock = new(new Instant());
    private static string? _jwt;
    private static LeaderboardViewModel _createdLeaderboard = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _factory = new TestApiFactory().WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
                services.AddSingleton<IClock, FakeClock>(_ => _clock)
            )
        );
        _apiClient = new TestApiClient(_factory.CreateClient());

        PostgresDatabaseFixture.ResetDatabaseToTemplate();
        _jwt = (await _apiClient.LoginAdminUser()).Token;

        _createdLeaderboard = await _apiClient.Post<LeaderboardViewModel>(
            "/leaderboards/create",
            new()
            {
                Body = new CreateLeaderboardRequest()
                {
                    Name = "Super Mario Bros.",
                    Slug = "super_mario_bros",
                },
                Jwt = _jwt
            }
        );
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => _factory.Dispose();

    [Test]
    public static async Task GetCategory_NotFound() =>
        await _apiClient.Awaiting(
            a => a.Get<CategoryViewModel>(
                $"/api/cateogries/69",
                new() { Jwt = _jwt }
            )
        ).Should()
        .ThrowAsync<RequestFailureException>()
        .Where(e => e.Response.StatusCode == HttpStatusCode.NotFound);

    [Test]
    public static async Task CreateCategory_GetCategory_OK()
    {
        Instant now = Instant.FromUnixTimeSeconds(1);
        _clock.Reset(now);

        CreateCategoryRequest request = new()
        {
            Name = "1 Player",
            Slug = "1_player",
            Info = "only one guy allowed",
            SortDirection = SortDirection.Ascending,
            Type = RunType.Time
        };

        CategoryViewModel createdCategory = await _apiClient.Post<CategoryViewModel>(
            $"/leaderboard/{_createdLeaderboard.Id}/categories/create",
            new()
            {
                Body = request,
                Jwt = _jwt
            }
        );

        createdCategory.CreatedAt.Should().Be(now);

        CategoryViewModel retrievedCategory = await _apiClient.Get<CategoryViewModel>(
            $"/api/category/{createdCategory?.Id}", new() { }
        );

        retrievedCategory.Should().BeEquivalentTo(request);
    }

    [Test]
    public static async Task CreateCategory_Unauthenticated()
    {
        Instant now = Instant.FromUnixTimeSeconds(1);
        _clock.Reset(now);

        CreateCategoryRequest request = new()
        {
            Name = "1 Player",
            Slug = "1_player",
            Info = "only one guy allowed",
            SortDirection = SortDirection.Ascending,
            Type = RunType.Time
        };

        await FluentActions.Awaiting(() => _apiClient.Post<CategoryViewModel>(
            $"/leaderboard/{_createdLeaderboard.Id}/categories/create",
            new()
            {
                Body = request,
            }
        )).Should().ThrowAsync<RequestFailureException>().Where(e => e.Response.StatusCode == HttpStatusCode.Unauthorized);
    }

    [TestCase(UserRole.Banned)]
    [TestCase(UserRole.Confirmed)]
    [TestCase(UserRole.Registered)]
    public static async Task CreateCategory_BadRole(UserRole role)
    {
        IServiceScope scope = _factory.Services.CreateScope();
        IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        string email = $"testuser.updatelb.{role}@example.com";

        RegisterRequest registerRequest = new()
        {
            Email = email,
            Password = "Passw0rd",
            Username = $"CreateCatTest{role}"
        };

        await userService.CreateUser(registerRequest);
        LoginResponse res = await _apiClient.LoginUser(registerRequest.Email, registerRequest.Password);

        CreateCategoryRequest request = new()
        {
            Name = "1 Player",
            Slug = "1_player",
            Info = "only one guy allowed",
            SortDirection = SortDirection.Ascending,
            Type = RunType.Time
        };

        await FluentActions.Awaiting(() => _apiClient.Post<CategoryViewModel>(
            $"/leaderboard/{_createdLeaderboard.Id}/categories/create",
            new()
            {
                Body = request,
                Jwt = res.Token,
            }
        )).Should().ThrowAsync<RequestFailureException>().Where(e => e.Response.StatusCode == HttpStatusCode.Forbidden);
    }

    [Test]
    public static async Task CreateCategory_LeaderboardNotFound()
    {
        CreateCategoryRequest request = new()
        {
            Name = "1 Player",
            Slug = "1_player",
            Info = "only one guy allowed",
            SortDirection = SortDirection.Ascending,
            Type = RunType.Time
        };

        ExceptionAssertions<RequestFailureException> exAssert = await FluentActions.Awaiting(() => _apiClient.Post<CategoryViewModel>(
            "/leaderboard/1000/categories/create",
            new()
            {
                Body = request,
                Jwt = _jwt,
            }
        )).Should().ThrowAsync<RequestFailureException>().Where(e => e.Response.StatusCode == HttpStatusCode.NotFound);

        ProblemDetails? problemDetails = await exAssert.Which.Response.Content.ReadFromJsonAsync<ProblemDetails>(TestInitCommonFields.JsonSerializerOptions);
        problemDetails.Should().NotBeNull();
        problemDetails!.Title.Should().Be("Leaderboard Not Found");
    }

    [Test]
    public static async Task CreateCategory_NoConflictBecauseOldCatIsDeleted()
    {
        ApplicationContext context = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationContext>();

        Category cat = new()
        {
            Name = "First",
            Slug = "should-not-conflict",
            LeaderboardId = _createdLeaderboard.Id,
            SortDirection = SortDirection.Ascending,
            Type = RunType.Score,
            DeletedAt = _clock.GetCurrentInstant(),
        };

        context.Categories.Add(cat);
        await context.SaveChangesAsync();
        cat.Id.Should().NotBe(default);

        CreateCategoryRequest request = new()
        {
            Name = "1 Player",
            Slug = "should-not-conflict",
            Info = "only one guy allowed",
            SortDirection = SortDirection.Ascending,
            Type = RunType.Time
        };

        await FluentActions.Awaiting(() => _apiClient.Post<CategoryViewModel>(
            $"/leaderboard/{_createdLeaderboard.Id}/categories/create",
            new()
            {
                Body = request,
                Jwt = _jwt
            }
        )).Should().NotThrowAsync();
    }

    [Test]
    public static async Task CreateCategory_Conflict()
    {
        CreateCategoryRequest request = new()
        {
            Name = "First",
            Slug = "repeated-slug",
            Info = "only one guy allowed",
            SortDirection = SortDirection.Ascending,
            Type = RunType.Time
        };

        CategoryViewModel created = await _apiClient.Post<CategoryViewModel>(
            $"/leaderboard/{_createdLeaderboard.Id}/categories/create",
            new()
            {
                Body = request,
                Jwt = _jwt
            }
        );

        ExceptionAssertions<RequestFailureException> exAssert = await FluentActions.Awaiting(() => _apiClient.Post<CategoryViewModel>(
            $"/leaderboard/{_createdLeaderboard.Id}/categories/create",
            new()
            {
                Body = request,
                Jwt = _jwt,
            }
        )).Should().ThrowAsync<RequestFailureException>().Where(e => e.Response.StatusCode == HttpStatusCode.Conflict);

        CategoryViewModel? conflict = await exAssert.Which.Response.Content.ReadFromJsonAsync<CategoryViewModel>(TestInitCommonFields.JsonSerializerOptions);
        conflict.Should().Be(created);
    }

    [TestCase(null, "1_player")]
    [TestCase("1 Player", null)]
    // TODO: Figure out how to test against sort direction and run type. Passing
    // invalid values results in serialisation errors instead of 422s as expected.
    public static async Task CreateCategory_BadData(string? name, string? slug)
    {
        CreateCategoryRequest request = new()
        {
            Info = "only one guy allowed",
            SortDirection = SortDirection.Ascending,
            Type = RunType.Score,
        };

        if (name != null)
        {
            request.Name = name;
        }

        if (slug != null)
        {
            request.Slug = slug;
        }

        ExceptionAssertions<RequestFailureException> exAssert = await FluentActions.Awaiting(() => _apiClient.Post<CategoryViewModel>(
            $"/leaderboard/{_createdLeaderboard.Id}/categories/create",
            new()
            {
                Body = request,
                Jwt = _jwt,
            }
        )).Should().ThrowAsync<RequestFailureException>().Where(e => e.Response.StatusCode == HttpStatusCode.UnprocessableContent);

        ProblemDetails? problemDetails = await exAssert.Which.Response.Content.ReadFromJsonAsync<ProblemDetails>(TestInitCommonFields.JsonSerializerOptions);
        problemDetails.Should().NotBeNull();
        problemDetails!.Title.Should().Be("One or more validation errors occurred.");
    }
}
