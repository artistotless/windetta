using System.ComponentModel.DataAnnotations;

namespace Windetta.Identity.Mvc.Models;

public class InputEmailViewModel
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }

    public string ReturnUrl { get; set; }

    [Required]
    public string Provider { get; set; }
}
