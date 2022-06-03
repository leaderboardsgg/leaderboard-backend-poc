using System;
using System.Net;
using System.Threading.Tasks;
using LeaderboardBackend.Authorization;
using LeaderboardBackend.Models.Entities;
using LeaderboardBackend.Models.Requests;
using LeaderboardBackend.Test.Lib;
using LeaderboardBackend.Test.TestApi;
using LeaderboardBackend.Test.TestApi.Extensions;
using NUnit.Framework;


namespace LeaderboardBackend.Test;

[TestFixture]
internal class Bans
{
	private static TestApiFactory Factory = null!;
	private static TestApiClient ApiClient = null!;
	private static string? AdminJwt;
	private static Leaderboard DefaultLeaderboard = null!;
	private static User NormalUser = null!;
	private static User ModUser = null!;
	private static string? ModJwt;

	[SetUp]
	public static async Task SetUp()
	{
		Factory = new TestApiFactory();
		ApiClient = Factory.CreateTestApiClient();
		AdminJwt = (await ApiClient.LoginAdminUser()).Token;

		// Set up users and leaderboard
		NormalUser = await ApiClient.RegisterUser(
			"normal",
			"normal@email.com",
			"Passw0rd!"
		);
		ModUser = await ApiClient.RegisterUser(
			"mod",
			"mod@email.com",
			"Passw0rd!"
		);

		DefaultLeaderboard = await ApiClient.Post<Leaderboard>(
			"/api/leaderboards",
			new()
			{
				Body = new CreateLeaderboardRequest
				{
					Name = Generators.GenerateRandomString(),
					Slug = Generators.GenerateRandomString(),
				},
				Jwt = AdminJwt,
			}
		);

		await ApiClient.Post<Modship>(
			"/api/modships",
			new()
			{
				Body = new CreateModshipRequest
				{
					LeaderboardId = DefaultLeaderboard.Id,
					UserId = ModUser.Id,
				},
				Jwt = AdminJwt,
			}

		);

		ModJwt = (await ApiClient.LoginUser("mod@email.com", "Passw0rd!")).Token;
	}

	[Test]
	public static async Task CreateSiteBan_Ok()
	{
		Ban created = await CreateSiteBan(NormalUser.Id, "reason");
		Ban retrieved = await GetBan(created.Id);

		Assert.IsNotNull(created);
		Assert.AreEqual(retrieved.Id, created.Id);
		Assert.AreEqual(retrieved.BannedUserId, created.BannedUserId);
		Assert.AreEqual(retrieved.BanningUserId, created.BanningUserId);
		Assert.IsNull(retrieved.LeaderboardId);
	}

	[Test]
	public static async Task CreateSiteBan_TryBanAdmin_Unauthorized()
	{
		try
		{
			await CreateSiteBan(TestInitCommonFields.Admin.Id, "reason");
			Assert.Fail("You should not be able to ban an admin (yourself).");
		} catch (RequestFailureException e)
		{
			Assert.AreEqual(HttpStatusCode.Unauthorized, e.Response.StatusCode);
		}
	}

	[Test]
	public static async Task CreateLeaderboardBan_Ok()
	{
		Ban created = await CreateLeaderboardBan(NormalUser.Id, "reason");
		Ban retrieved = await GetBan(created.Id);

		Assert.IsNotNull(created);
		Assert.AreEqual(retrieved.Id, created.Id);
		Assert.AreEqual(retrieved.BannedUserId, created.BannedUserId);
		Assert.AreEqual(retrieved.BanningUserId, created.BanningUserId);
		Assert.AreEqual(retrieved.LeaderboardId, created.LeaderboardId);
	}

	public static async Task CreateLeaderboardBan_TryMod_Unauthorized()
	{
		try
		{
			await CreateLeaderboardBan(ModUser.Id, "reason");
			Assert.Fail("You should not be able to ban a mod (yourself).");
		} catch (RequestFailureException e)
		{
			Assert.AreEqual(HttpStatusCode.Unauthorized, e.Response.StatusCode);
		}
	}

	private static async Task<User> CreateUser(string username, string email, string password)
	{
		return await ApiClient.Post<User>(
			"/api/Users/register",
			new()
			{
				Body = new RegisterRequest()
				{
					Username = username,
					Email = email,
					Password = password,
					PasswordConfirm = password
				},
				Jwt = AdminJwt
			}
		);
	}

	private static async Task<Ban> CreateSiteBan(Guid userId, string reason)
	{
		return await ApiClient.Post<Ban>(
			"api/Bans",
			new()
			{
				Body = new CreateSiteBanRequest()
				{
					UserId = userId,
					Reason = reason
				},
				Jwt = AdminJwt
			}
		);
	}

	private static async Task<Ban> CreateLeaderboardBan(Guid userId, string reason)
	{
		return await ApiClient.Post<Ban>(
			"api/Bans/leaderboard",
			new()
			{
				Body = new CreateLeaderboardBanRequest()
				{
					UserId = userId,
					Reason = reason,
					LeaderboardId = DefaultLeaderboard.Id
				},
				Jwt = ModJwt
			}
		);
	}

	private static async Task<Ban> GetBan(long id)
	{
		return await ApiClient.Get<Ban>(
			$"api/Bans/{id}",
			new()
			{
				Jwt = AdminJwt
			}
		);
	}
}
