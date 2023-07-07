using System.ComponentModel.DataAnnotations;
using Windetta.Common.Messages;
using Windetta.Identity.Messages.Responses;

namespace Windetta.Identity.Messages.Requests;

public class LocalLoginRequest : IRequest<LocalLoginResponse>
{
    public string ReturnUrl { get; set; }

    public bool RememberLogin { get; set; }

    [MaxLength(25)]
    [Required]
    public string Username { get; set; }

    [MaxLength(100)]
    [Required]
    public string Password { get; set; }
}
