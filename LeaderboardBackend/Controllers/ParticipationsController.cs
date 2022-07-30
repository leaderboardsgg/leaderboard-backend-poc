using LeaderboardBackend.Controllers.Annotations;
using LeaderboardBackend.Models.Entities;
using LeaderboardBackend.Models.Requests;
using LeaderboardBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaderboardBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParticipationsController : ControllerBase
{
	private readonly IAuthService _authService;
	private readonly IParticipationService _participationService;
	private readonly IRunService _runService;
	private readonly IUserService _userService;

	public ParticipationsController(
		IAuthService authService,
		IParticipationService participationService,
		IRunService runService,
		IUserService userService)
	{
		_authService = authService;
		_participationService = participationService;
		_runService = runService;
		_userService = userService;
	}

	/// <summary>
	///     Gets a participation for a run.
	/// </summary>
	/// <param name="id">The participation ID.</param>
	/// <response code="200">The participation object.</response>
	/// <response code="404">No participations found.</response>
	[AllowAnonymous]
	[ApiConventionMethod(typeof(Conventions), nameof(Conventions.GetAnon))]
	[HttpGet("{id}")]
	public async Task<ActionResult<Participation>> GetParticipation(long id)
	{
		Participation? participation = await _participationService.GetParticipation(id);
		if (participation == null)
		{
			return NotFound();
		}

		return Ok(participation);
	}

	/// <summary>
	///     Creates a participation of a user for a run.
	/// </summary>
	/// <param name="request">The request body.</param>
	/// <response code="201">The newly-created participation object.</response>
	/// <response code="404">Either the runner or run could not be found.</response>
	[ApiConventionMethod(typeof(Conventions), nameof(Conventions.Post))]
	[HttpPost]
	public async Task<ActionResult> CreateParticipation(
		[FromBody] CreateParticipationRequest request)
	{
		// TODO: Maybe create validation middleware
		if (request.IsSubmitter && (request.Comment is null || request.Vod is null))
		{
			return BadRequest();
		}

		User? runner = await _userService.GetUserById(request.RunnerId);
		Run? run = await _runService.GetRun(request.RunId);

		// FIXME: runner null check should probably 500 if it equals the caller's ID. In fact, we
		// might want to review this method. It's pretty weird. - zysim
		if (runner is null || run is null)
		{
			return NotFound();
		}

		Participation participation = new()
		{
			Comment = request.Comment,
			RunId = request.RunId,
			Run = run,
			RunnerId = request.RunnerId,
			Runner = runner,
			Vod = request.Vod
		};

		await _participationService.CreateParticipation(participation);

		return CreatedAtAction(
			nameof(GetParticipation),
			new { id = participation.Id },
			participation);
	}

	/// <summary>
	///     Updates the participation of a user for a run.
	/// </summary>
	/// <remarks>Expects both a comment and a VoD link.</remarks>
	/// <param name="request">The request body.</param>
	/// <response code="200">A successful update.</response>
	/// <response code="404">The participation could not be found.</response>
	[ApiConventionMethod(typeof(Conventions), nameof(Conventions.Update))]
	[Authorize]
	[HttpPut]
	public async Task<ActionResult> UpdateParticipation(
		[FromBody] UpdateParticipationRequest request)
	{
		// FIXME: We should review this method. The way it handles things is pretty weird. For
		// example we may want to allow updating other users' participations, on top of the fact
		// that users may have multiple participations, which we should also handle. - zysim
		Guid? userId = _authService.GetUserIdFromClaims(HttpContext.User);

		if (userId is null)
		{
			return Forbid();
		}

		Participation? participation = await _participationService
			.GetParticipationForUser(userId.Value);

		if (participation is null)
		{
			return NotFound();
		}

		participation.Comment = request.Comment;
		participation.Vod = request.Vod;

		await _participationService.UpdateParticipation(participation);

		return Ok();
	}
}
