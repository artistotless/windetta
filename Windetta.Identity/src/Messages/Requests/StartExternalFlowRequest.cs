using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Messages;

namespace Windetta.Identity.Messages.Requests;

public class StartExternalFlowRequest : IRequest<AuthenticationProperties>
{
    public string Scheme { get; set; }
    public string ReturnUrl { get; set; }
}

public class StartExternalFlowHandler : IRequestHandler<StartExternalFlowRequest, AuthenticationProperties>
{
    private readonly IUrlHelper _urlHelper;
    private readonly IIdentityServerInteractionService _interaction;

    public StartExternalFlowHandler(IUrlHelper urlHelper,
        IIdentityServerInteractionService interaction)
    {
        _urlHelper = urlHelper;
        _interaction = interaction;
    }

    public Task<AuthenticationProperties> HandleAsync(StartExternalFlowRequest request)
    {
        var returnUrl = request.ReturnUrl;

        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = "~/";

        // validate returnUrl - either it is a valid OIDC URL or back to a local page
        if (_urlHelper.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
            // user might have clicked on a malicious link - should be logged
            throw new Exception("invalid return URL");

        // start challenge and roundtrip the return URL and scheme 
        return Task.FromResult(new AuthenticationProperties
        {
            RedirectUri = _urlHelper.Action("Callback"),
            Items = { { "returnUrl", returnUrl }, { "scheme", request.Scheme }, }
        });
    }
}