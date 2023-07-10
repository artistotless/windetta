using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Windetta.Identity.Models;

public class RegisterInputModel
{
    [Required]
    [MaxLength(40)]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain alphanumeric characters.")]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [PasswordPropertyText]
    [MaxLength(100)]
    public string Password { get; set; }

    [Required]
    [PasswordPropertyText]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}