using System.Security.Claims;
using System.Security.Principal;
using Windetta.Common.Messages;

namespace Windetta.Identity.Messages.Comands;

public class ExternalLogin : IRequest
{
    public ClaimsIdentity Identity { get; set; }
    public string Provider { get; set; }
    public string ReturnUrl { get; set; }
}
