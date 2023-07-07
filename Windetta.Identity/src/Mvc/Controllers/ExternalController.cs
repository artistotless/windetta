using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Windetta.Common.Types;
using Windetta.Identity.Controllers;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Services;

namespace Windetta.Identity.Mvc.Controllers;

public class ExternalController : BaseController
{
    private readonly IRequestDispatcher _dispatcher;

    public ExternalController(IRequestDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Authenticate user using external auth provider
    /// </summary>
    [HttpGet]
    [Route("signin/{provider}")]
    public async Task SignInWithExternal(string provider, string returnUrl)
        => await ChallengeAsync(provider.ToLower(), returnUrl);

    /// <summary>
    /// Handle the response recieved from external Oauth provider
    /// </summary>
    [HttpGet]
    [Route("signin/{provider}/callback")]
    public async Task<IActionResult> ExternalSignInCallback(string provider, string returnUrl)
    {
        var authentication = await HttpContext.AuthenticateAsync(provider);

        // Authentication failed
        if (authentication is null || !authentication.Succeeded)
            return Unauthorized();

        // Authentication failed
        if (authentication.Principal is null || authentication.Principal.Identity is null)
            throw new WindettaException("Fetching data from external OpenID provider failed");

        var identity = authentication.Principal.Identity;
        var command = new ExternalLoginRequest()
        {
            Provider = provider,
            Claims = (identity as ClaimsIdentity)!.Claims,
            ReturnUrl = returnUrl
        };

        // Authentication passed.
        // Return authCode across api gateway to end cliend.
        var redirectUrl = await _dispatcher.HandleAsync(command);

        return new RedirectResult(redirectUrl);
    }

    /// <summary>
    /// Redirect to external OpenID page
    /// </summary>
    private Task ChallengeAsync(string scheme, string returnUrl)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(ExternalSignInCallback),
            new { returnUrl = returnUrl, provider = scheme })?.ToLower()
        };

        return HttpContext.ChallengeAsync(scheme, properties);
    }
}
