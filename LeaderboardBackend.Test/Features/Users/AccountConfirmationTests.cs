using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LeaderboardBackend.Models.Entities;
using LeaderboardBackend.Services;
using LeaderboardBackend.Test.Fixtures;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NodaTime;
using NodaTime.Testing;
using NUnit.Framework;

namespace LeaderboardBackend.Test.Features.Users;

[TestFixture]
public class AccountConfirmationTests : IntegrationTestsBase
{
    private IServiceScope _scope = null!;
    private IAuthService _authService = null!;

    [SetUp]
    public void Init()
    {
        _scope = _factory.Services.CreateScope();
        _authService = _scope.ServiceProvider.GetRequiredService<IAuthService>();
    }

    [TearDown]
    public void TearDown()
    {
        _factory.ResetDatabase();
        _scope.Dispose();
    }

    [Test]
    public async Task ResendConfirmation_Unauthorised()
    {
        HttpResponseMessage res = await Client.PostAsync(Routes.RESEND_CONFIRMATION, null);
        res.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task ResendConfirmation_NotFound_ShouldGet401()
    {
        string token = _authService.GenerateJSONWebToken(new()
        {
            Email = "unknown@user.com",
            Password = "password",
            Username = "username",
        });

        Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");
        HttpResponseMessage res = await Client.PostAsync(Routes.RESEND_CONFIRMATION, null);

        res.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task ResendConfirmation_Conflict()
    {
        // TODO: Call UserService instead, once we're able to set a user's role with it.
        ApplicationContext context = _scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        User user = new()
        {
            Email = "test@email.com",
            Password = "password",
            Username = "username",
            Role = UserRole.Confirmed,
        };
        context.Add<User>(user);
        context.SaveChanges();
        string token = _authService.GenerateJSONWebToken(user);

        Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");
        HttpResponseMessage res = await Client.PostAsync(Routes.RESEND_CONFIRMATION, null);

        res.Should().HaveStatusCode(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task ResendConfirmation_EmailFailedToSend()
    {
        Mock<IEmailSender> emailSenderMock = new();
        emailSenderMock.Setup(e =>
            e.EnqueueEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
        ).Throws(new System.Exception());
        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<IEmailSender>(_ => emailSenderMock.Object);
            });
        })
        .CreateClient();

        IUserService userService = _scope.ServiceProvider.GetRequiredService<IUserService>();
        CreateUserResult result = await userService.CreateUser(new()
        {
            Email = "test@email.com",
            Password = "password",
            Username = "username",
        });
        string token = _authService.GenerateJSONWebToken(result.AsT0);

        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");
        HttpResponseMessage res = await client.PostAsync(Routes.RESEND_CONFIRMATION, null);
        res.Should().HaveStatusCode(HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task ResendConfirmation_Success()
    {
        Mock<IEmailSender> emailSenderMock = new();
        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<IEmailSender>(_ => emailSenderMock.Object);
                services.AddSingleton<IClock, FakeClock>(_ => new(Instant.FromUnixTimeSeconds(1)));
            });
        })
        .CreateClient();
        IUserService userService = _scope.ServiceProvider.GetRequiredService<IUserService>();
        CreateUserResult result = await userService.CreateUser(new()
        {
            Email = "test@email.com",
            Password = "password",
            Username = "username",
        });
        string token = _authService.GenerateJSONWebToken(result.AsT0);

        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");
        HttpResponseMessage res = await client.PostAsync(Routes.RESEND_CONFIRMATION, null);
        res.Should().HaveStatusCode(HttpStatusCode.OK);
        emailSenderMock.Verify(x =>
            x.EnqueueEmailAsync(
                "test@email.com",
                "Confirm Your Account",
                It.IsAny<string>()
            ),
            Times.Once()
        );

        ApplicationContext context = _scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        AccountConfirmation confirmation = context.AccountConfirmations.First(c => c.UserId == result.AsT0.Id);
        confirmation.Should().NotBeNull();
        confirmation.CreatedAt.ToUnixTimeSeconds().Should().Be(1);
        confirmation.UsedAt.Should().BeNull();
        Instant.Subtract(confirmation.ExpiresAt, confirmation.CreatedAt).Should().Be(Duration.FromHours(1));
    }

    [Test]
    public async Task ConfirmAccount_BadConfirmationId()
    {
        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IClock, FakeClock>(_ => new(Instant.FromUnixTimeSeconds(1)));
            });
        }).CreateClient();

        IUserService userService = _scope.ServiceProvider.GetRequiredService<IUserService>();

        CreateUserResult result = await userService.CreateUser(new()
        {
            Email = "test@email.com",
            Password = "password",
            Username = "username",
        });

        User user = result.AsT0;
        ApplicationContext context = _scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        await context.AccountConfirmations.AddAsync(new AccountConfirmation()
        {
            CreatedAt = Instant.FromUnixTimeSeconds(0),
            ExpiresAt = Instant.FromUnixTimeSeconds(0).Plus(Duration.FromHours(1)),
            UserId = user.Id
        });

        await context.SaveChangesAsync();

        HttpResponseMessage res = await client.PutAsync(Routes.ConfirmAccount(Guid.NewGuid()), null);
        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
        user.Role.Should().Be(UserRole.Registered);
    }

    [Test]
    public async Task ConfirmAccount_BadRole()
    {
        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IClock, FakeClock>(_ => new(Instant.FromUnixTimeSeconds(1)));
            });
        }).CreateClient();

        IUserService userService = _scope.ServiceProvider.GetRequiredService<IUserService>();

        CreateUserResult result = await userService.CreateUser(new()
        {
            Email = "test@email.com",
            Password = "password",
            Username = "username",
        });

        User user = result.AsT0;
        user.Role = UserRole.Confirmed;
        ApplicationContext context = _scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        AccountConfirmation confirmation = new()
        {
            CreatedAt = Instant.FromUnixTimeSeconds(0),
            ExpiresAt = Instant.FromUnixTimeSeconds(0).Plus(Duration.FromHours(1)),
            UserId = user.Id
        };

        await context.AccountConfirmations.AddAsync(confirmation);
        await context.SaveChangesAsync();

        HttpResponseMessage res = await client.PutAsync(Routes.ConfirmAccount(confirmation.Id), null);
        res.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task ConfirmAccount_Expired()
    {
        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IClock, FakeClock>(_ => new(Instant.FromUnixTimeSeconds(1).Plus(Duration.FromHours(2))));
            });
        }).CreateClient();

        IUserService userService = _scope.ServiceProvider.GetRequiredService<IUserService>();

        CreateUserResult result = await userService.CreateUser(new()
        {
            Email = "test@email.com",
            Password = "password",
            Username = "username",
        });

        User user = result.AsT0;
        ApplicationContext context = _scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        AccountConfirmation confirmation = new()
        {
            CreatedAt = Instant.FromUnixTimeSeconds(0),
            ExpiresAt = Instant.FromUnixTimeSeconds(0).Plus(Duration.FromHours(1)),
            UserId = user.Id
        };

        await context.AccountConfirmations.AddAsync(confirmation);
        await context.SaveChangesAsync();

        HttpResponseMessage res = await client.PutAsync(Routes.ConfirmAccount(confirmation.Id), null);
        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
        user.Role.Should().Be(UserRole.Registered);
    }

    [Test]
    public async Task ConfirmAccount_AlreadyUsed()
    {
        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IClock, FakeClock>(_ => new(Instant.FromUnixTimeSeconds(1)));
            });
        }).CreateClient();

        IUserService userService = _scope.ServiceProvider.GetRequiredService<IUserService>();

        CreateUserResult result = await userService.CreateUser(new()
        {
            Email = "test@email.com",
            Password = "password",
            Username = "username",
        });

        User user = result.AsT0;
        ApplicationContext context = _scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        AccountConfirmation confirmation = new()
        {
            CreatedAt = Instant.FromUnixTimeSeconds(0),
            ExpiresAt = Instant.FromUnixTimeSeconds(0).Plus(Duration.FromHours(1)),
            UsedAt = Instant.FromUnixTimeSeconds(5),
            UserId = user.Id
        };

        await context.AccountConfirmations.AddAsync(confirmation);
        await context.SaveChangesAsync();

        HttpResponseMessage res = await client.PutAsync(Routes.ConfirmAccount(confirmation.Id), null);
        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
        user.Role.Should().Be(UserRole.Registered);
    }

    [Test]
    public async Task ConfirmAccount_Success()
    {
        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IClock, FakeClock>(_ => new(Instant.FromUnixTimeSeconds(1)));
            });
        }).CreateClient();

        AccountConfirmation confirmation = new()
        {
            CreatedAt = Instant.FromUnixTimeSeconds(0),
            ExpiresAt = Instant.FromUnixTimeSeconds(0).Plus(Duration.FromHours(1)),
            User = new()
            {
                Email = "test@email.com",
                Password = "password",
                Username = "username",
            }
        };

        ApplicationContext context = _scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        await context.AccountConfirmations.AddAsync(confirmation);
        await context.SaveChangesAsync();
        HttpResponseMessage res = await client.PutAsync(Routes.ConfirmAccount(confirmation.Id), null);
        res.Should().HaveStatusCode(HttpStatusCode.OK);
        // AccountConfirmation? conf = await context.AccountConfirmations.Include(c => c.User).SingleOrDefaultAsync(c => c.Id == confirmation.Id);
        // conf!.User.Role.Should().Be(UserRole.Confirmed);
        // conf!.UsedAt.Should().NotBeNull();
        confirmation.UsedAt.Should().NotBeNull();
        confirmation.User.Role.Should().Be(UserRole.Confirmed);
    }
}
