using Windetta.Common.Messages;
using Windetta.Identity.Models;

namespace Windetta.Identity.Messages.Requests;

public class StartLoginFlowRequest : IRequest<LoginViewModel>
{
    public string ReturnUrl { get; set; }
}
