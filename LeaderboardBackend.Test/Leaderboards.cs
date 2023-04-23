using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LeaderboardBackend.Models.Requests;
using LeaderboardBackend.Models.ViewModels;
using LeaderboardBackend.Test.Lib;
using LeaderboardBackend.Test.TestApi;
using LeaderboardBackend.Test.TestApi.Extensions;
using NUnit.Framework;

namespace LeaderboardBackend.Test;

[TestFixture]
internal class Leaderboards
{
	private static TestApiClient s_apiClient = null!;
	private static TestApiFactory s_factory = null!;
	private static string? s_jwt;

	[OneTimeSetUp]
	public async Task OneTimeSetUp()
	{
		s_factory = new TestApiFactory();
		s_apiClient = s_factory.CreateTestApiClient();

		s_factory.ResetDatabase();
		s_jwt = (await s_apiClient.LoginAdminUser()).Token;
	}

	[OneTimeTearDown]
	public void OneTimeTearDown()
	{
		s_factory.Dispose();
	}

	[Test]
	public static void GetLeaderboard_NotFound()
	{
		RequestFailureException e = Assert.ThrowsAsync<RequestFailureException>(async () =>
			await s_apiClient.Get<LeaderboardViewModel>($"/api/leaderboards/{long.MaxValue}", new()))!;

		Assert.AreEqual(HttpStatusCode.NotFound, e.Response.StatusCode);
	}

	[Test]
	public static async Task CreateLeaderboard_GetLeaderboard_OK()
	{
		LeaderboardViewModel createdLeaderboard = await s_apiClient.Post<LeaderboardViewModel>(
			"/api/leaderboards",
			new()
			{
				Body = new CreateLeaderboardRequest
				{
					Name = Generators.GenerateRandomString(),
					Slug = Generators.GenerateRandomString()
				},
				Jwt = s_jwt
			});

		LeaderboardViewModel retrievedLeaderboard = await s_apiClient.Get<LeaderboardViewModel>(
			$"/api/leaderboards/{createdLeaderboard?.Id}",
			new());

		Assert.AreEqual(createdLeaderboard, retrievedLeaderboard);
	}

	[Test]
	public static async Task CreateLeaderboards_GetLeaderboards()
	{
		HashSet<LeaderboardViewModel> createdLeaderboards = new();

		for (int i = 0; i < 5; i++)
		{
			createdLeaderboards.Add(
				await s_apiClient.Post<LeaderboardViewModel>(
					"/api/leaderboards",
					new()
					{
						Body = new CreateLeaderboardRequest
						{
							Name = Generators.GenerateRandomString(),
							Slug = Generators.GenerateRandomString()
						},
						Jwt = s_jwt
					}));
		}

		IEnumerable<long> leaderboardIds = createdLeaderboards.Select(l => l.Id).ToList();
		string leaderboardIdQuery = ListToQueryString(leaderboardIds, "ids");

		List<LeaderboardViewModel> leaderboards = await s_apiClient.Get<List<LeaderboardViewModel>>(
			$"api/leaderboards?{leaderboardIdQuery}",
			new());

		foreach (LeaderboardViewModel leaderboard in leaderboards)
		{
			Assert.IsTrue(createdLeaderboards.Contains(leaderboard));
			createdLeaderboards.Remove(leaderboard);
		}

		Assert.AreEqual(0, createdLeaderboards.Count);
	}

	private static string ListToQueryString<T>(IEnumerable<T> list, string key)
	{
		IEnumerable<string> queryList = list.Select(l => $"{key}={l}");
		return string.Join("&", queryList);
	}
}
