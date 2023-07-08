using System.ComponentModel.DataAnnotations;

namespace Windetta.Identity.Mvc.Models;

public class ExternalSignInCallbackModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public string? ReturnUrl { get; set; }

    [Required]
    public string Provider { get; set; }
}
