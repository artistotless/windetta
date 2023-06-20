using System.ComponentModel.DataAnnotations;
using Windetta.Common.Authentication;
using Windetta.Common.Messages;

namespace Windetta.Identity.Messages.Requests;

public class LoginRequest : IRequest<JsonWebTokenBase>
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [MaxLength(100)]
    [Required]
    public string Password { get; set; }
}
