using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Types;
using Windetta.Identity.Controllers;
using Windetta.Identity.Extensions;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Mvc.Models;
using Windetta.Identity.Services;

namespace Windetta.Identity.Mvc.Controllers;

[Route("[controller]")]
public class ExternalController : BaseController
{
    private readonly IRequestDispatcher _dispatcher;

    public ExternalController(IRequestDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Authenticate user using external auth provider
    /// Redirects to external OIDC page
    /// </summary>
    [HttpGet]
    [Route("{provider}")]
    public async Task ExternalSignIn(string provider, string? returnUrl = null)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(ExternalSignInCallback),
            new { returnUrl = returnUrl, provider = provider.ToLower() })?.ToLower()
        };

        await HttpContext.ChallengeAsync(provider.ToLower(), properties);
    }

    /// <summary>
    /// Handle the response recieved from external Oauth provider
    /// </summary>
    [HttpGet]
    [Route("{provider}/callback")]
    public async Task<IActionResult> ExternalSignInCallback([FromRoute] ExternalSignInCallbackModel model)
    {
        var authResult = await HttpContext
            .AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        // Authentication failed
        if (authResult is null || !authResult.Succeeded)
            return Unauthorized();

        // Authentication failed
        if (authResult.Principal is null || authResult.Principal.Identity is null)
            throw new WindettaException("Fetching data from external OpenID provider failed");

        var externalIdentity = await _dispatcher.HandleAsync(
            new ParseExternalIdentityRequest()
            {
                AuthResult = authResult,
                Provider = model.Provider,
            });

        // if external oidc provider does not return email
        // show input email page
        if ((externalIdentity.Email = model.Email ?? externalIdentity.Email) is null)
            return View("InputEmail", model);

        var request = new ExternalLoginRequest()
        {
            Provider = model.Provider,
            Identity = externalIdentity,
            ReturnUrl = model.ReturnUrl,
            Email = model.Email,
        };

        var context = await _dispatcher.HandleAsync(request);

        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        if (context != null)
        {
            if (context.IsNativeClient())
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage("Redirect", request.ReturnUrl);
            }
        }

        return new RedirectResult(request.ReturnUrl);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> InputEmail([FromForm] ExternalSignInCallbackModel model)
    {
        if (!ModelState.IsValid)
            return View("InputEmail", model);

        var existUserWithEmail = await _dispatcher.HandleAsync(
            new ExistUserWithEmailRequest()
            {
                Email = model.Email
            });

        if (existUserWithEmail is true)
        {
            ModelState.AddModelError(nameof(model.Email), "The email is already registered");
            return View("InputEmail", model);
        }

        return await ExternalSignInCallback(model);
    }
}
