using LeaderboardBackend.Controllers.Annotations;
using LeaderboardBackend.Models.Entities;
using LeaderboardBackend.Models.Requests;
using LeaderboardBackend.Models.ViewModels;
using LeaderboardBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BCryptNet = BCrypt.Net.BCrypt;

namespace LeaderboardBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public UsersController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    /// <summary>
    ///     Gets a User by their ID.
    /// </summary>
    /// <param name="id">The ID of the `User` which should be retrieved.</param>
    /// <response code="200">The `User` was found and returned successfully.</response>
    /// <response code="404">No `User` with the requested ID could be found.</response>
    [AllowAnonymous]
    [ApiConventionMethod(typeof(Conventions), nameof(Conventions.GetAnon))]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserViewModel>> GetUserById(Guid id)
    {
        User? user = await _userService.GetUserById(id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(UserViewModel.MapFrom(user));
    }

    /// <summary>
    ///     Logs a User in.
    /// </summary>
    /// <param name="request">
    ///     The `LoginRequest` instance from which to perform the login.
    /// </param>
    /// <response code="200">
    ///     The `User` was logged in successfully. A `LoginResponse` is returned.
    /// </response>
    /// <response code="400">The request was malformed.</response>
    /// <response code="401">The password passed was incorrect.</response>
    /// <response code="404">No `User` with the requested details could be found.</response>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        // FIXME: Use ApiConventionMethod here! - Ero

        User? user = await _userService.GetUserByEmail(request.Email);

        if (user is null)
        {
            return NotFound();
        }

        if (!BCryptNet.EnhancedVerify(request.Password, user.Password))
        {
            return Unauthorized();
        }

        string token = _authService.GenerateJSONWebToken(user);

        return Ok(new LoginResponse { Token = token });
    }

    /// <summary>
    ///     Gets the currently logged-in User.
    /// </summary>
    /// <remarks>
    ///     Call this method with the 'Authorization' header. A valid JWT bearer token must be
    ///     passed.<br/>
    ///     Example: `{ 'Authorization': 'Bearer JWT' }`.
    /// </remarks>
    /// <response code="200">The `User` was found and returned successfully..</response>
    /// <response code="403">An invalid JWT was passed in.</response>
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserViewModel>> Me()
    {
        // FIXME: Use ApiConventionMethod here! - Ero

        string? email = _authService.GetEmailFromClaims(HttpContext.User);

        if (email is null)
        {
            return Forbid();
        }

        User? user = await _userService.GetUserByEmail(email);

        // FIXME: Should return NotFound()! - Ero
        if (user is null)
        {
            return Forbid();
        }

        return Ok(UserViewModel.MapFrom(user));
    }
}
