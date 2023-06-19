using System.Security.Claims;
using Windetta.Common.Messages;

namespace Windetta.Identity.Messages.Requests;

public class ExternalLogin : IRequest
{
    public ClaimsIdentity Identity { get; set; }
    public string Provider { get; set; }
    public string ReturnUrl { get; set; }
}
