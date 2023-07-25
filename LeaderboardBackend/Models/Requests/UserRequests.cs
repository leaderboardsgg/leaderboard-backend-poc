using System.ComponentModel.DataAnnotations;
using FluentValidation;
using LeaderboardBackend.Models.Validation;

namespace LeaderboardBackend.Models.Requests;

/// <summary>
///     This request object is sent when a `User` is attempting to log in.
/// </summary>
public record LoginRequest
{
    // We use the null coalescing operators in both properties to achieve two things:
    // 1. Allow null-validation to be done in LoginRequestValidator below, to get the
    //    error format we want
    // 2. Prevent warning hints in code anywhere else of potentially-null values (we've
    //    verified that they won't be in LoginRequestValidator)
    // - zysim

    /// <summary>
    ///     The `User`'s email address.
    /// </summary>
    /// <example>john.doe@example.com</example>
    [EmailAddress]
    [Required]
    public string Email { get; set; } = null!;

    /// <summary>
    ///     The `User`'s password. It:
    ///     <ul>
    ///       <li>supports Unicode;</li>
    ///       <li>must be [8, 80] in length;</li>
    ///       <li>must have at least:</li>
    ///         <ul>
    ///           <li>one uppercase letter;</li>
    ///           <li>one lowercase letter; and</li>
    ///           <li>one number.</li>
    ///         </ul>
    ///     </ul>
    /// </summary>
    /// <example>P4ssword</example>
    [Required]
    public string Password { get; set; } = null!;
}

/// <summary>
///     This response object is received upon a successful log-in request.
/// </summary>
public record LoginResponse
{
    /// <summary>
    ///     A JSON Web Token to authenticate and authorize queries with.
    /// </summary>
    [Required]
    public string Token { get; set; } = null!;
}

/// <summary>
///     This request object is sent when a `User` is attempting to register.
/// </summary>
public record RegisterRequest
{
    /// <summary>
    ///     The username of the `User`. It:
    ///     <ul>
    ///       <li>must be [2, 25] in length;</li>
    ///       <li>must be made up of letters sandwiching zero or one of:</li>
    ///       <ul>
    ///         <li>hyphen;</li>
    ///         <li>underscore; or</li>
    ///         <li>apostrophe</li>
    ///       </ul>
    ///     </ul>
    ///     Usernames are saved case-sensitively, but matched against case-insensitively.
    ///     A `User` may not register with the name 'Cool' when another `User` with the name 'cool'
    ///     exists.
    /// </summary>
    /// <example>J'on-Doe</example>
    public required string Username { get; set; }

    /// <summary>
    ///     The `User`'s email address.
    /// </summary>
    /// <example>john.doe@example.com</example>
    public required string Email { get; set; }

    /// <summary>
    ///     The `User`'s password. It:
    ///     <ul>
    ///       <li>supports Unicode;</li>
    ///       <li>must be [8, 80] in length;</li>
    ///       <li>must have at least:</li>
    ///         <ul>
    ///           <li>one uppercase letter;</li>
    ///           <li>one lowercase letter; and</li>
    ///           <li>one number.</li>
    ///         </ul>
    ///     </ul>
    /// </summary>
    /// <example>P4ssword</example>
    public required string Password { get; set; }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        // NotNull() call needed to get the error message that NotNull creates
        RuleFor(x => x.Email).Cascade(CascadeMode.Stop).NotNull().EmailAddress();
        RuleFor(x => x.Password).UserPassword();
    }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username).Username();
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Password).UserPassword();
    }
}
