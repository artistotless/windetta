using Microsoft.AspNetCore.Authentication;
using Windetta.Common.Messages;

namespace Windetta.Identity.Messages.Requests;

public class StartExternalFlowRequest : IRequest<AuthenticationProperties>
{
    public string Scheme { get; set; }
    public string ReturnUrl { get; set; }
}
