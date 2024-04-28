using System.ComponentModel.DataAnnotations;

namespace Windetta.Identity.Models;

public class LoginInputModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    public bool RememberLogin { get; set; }
    public string ReturnUrl { get; set; }
}