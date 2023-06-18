using System.Security.Principal;
using Windetta.Common.Messages;

namespace Windetta.Identity.Messages.Comands;

public class ExternalLogin : IRequest
{
    public IIdentity Identity { get; set; }
    public string Provider { get; set; }
    public string ReturnUrl { get; set; }
}
