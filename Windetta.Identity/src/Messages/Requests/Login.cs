using System.ComponentModel.DataAnnotations;
using Windetta.Common.Messages;

namespace Windetta.Identity.Messages.Requests;

public class Login : IRequest
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [MaxLength(100)]
    [Required]
    public string Password { get; set; }
}
