using System.ComponentModel.DataAnnotations;
using LeaderboardBackend.Controllers.Requests;

namespace LeaderboardBackend.Models;

public class User
{
	[Required] public Guid Id { get; set; }

	[Required]
	[RegularExpression(@"^([a-zA-Z][-_']?){1,12}[a-zA-Z]$",
		ErrorMessage = "Your name must be between 2 and 25 characters, made up of letters sandwiching zero or one hyphen, underscore, or apostrophe.")]
	public string Username { get; set; } = null!;

	[Required]
	[EmailAddress]
	public string Email { get; set; } = null!;

	[Required]
	[Password]
	public string Password { get; set; } = null!;
}
