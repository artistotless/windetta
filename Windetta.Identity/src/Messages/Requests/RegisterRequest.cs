using System.ComponentModel.DataAnnotations;
using Windetta.Common.Messages;

namespace Windetta.Identity.Messages.Requests;

public class RegisterRequest : IRequest
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [Required]
    [MaxLength(30)]
    public string UserName { get; set; }

    [MaxLength(100)]
    [Required]
    public string Password { get; set; }
}
