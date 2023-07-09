using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Windetta.Common.Types;
using Windetta.Identity.Controllers;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Extensions;
using Windetta.Identity.Infrastructure.IdentityParsers;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Mvc.Models;
using Windetta.Identity.Services;

namespace Windetta.Identity.Mvc.Controllers;

[Route("[controller]")]
public class ExternalController : BaseController
{
    private readonly IRequestDispatcher _dispatcher;
    private readonly UserManager<User> _userManager;

    public ExternalController(IRequestDispatcher dispatcher, UserManager<User> userManager)
    {
        _dispatcher = dispatcher;
        this._userManager = userManager;
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

        var user = await _userManager.FindByLoginAsync(model.Provider, externalIdentity.UniqueId);
        if (user is null)
        {
            TempData.TryAdd("externalIdentity", JsonConvert.SerializeObject(externalIdentity));
            // if external oidc provider does not return email
            // show input email page
            if (externalIdentity.Email is null)
                return View("InputEmail", model);
        }

        return await ContinueExternalLogin(new ExternalLoginRequest()
        {
            Provider = model.Provider,
            Identity = externalIdentity,
            ReturnUrl = model.ReturnUrl,
        });
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> InputEmail([FromForm] InputEmailViewModel model)
    {
        if (!ModelState.IsValid)
            return View("InputEmail", model);

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is not null)
            ModelState.AddModelError(nameof(model.Email), "The email is already registered");

        if (!TempData.TryGetValue("externalIdentity", out var identityValue) || identityValue is null)
            ModelState.AddModelError(string.Empty, $"No identity info recieved from {model.Provider}");

        if (ModelState.ErrorCount > 0)
            return View("InputEmail", model);

        var identityObject = JsonConvert.DeserializeObject<ExternalIdentity>(identityValue.ToString());

        return await ContinueExternalLogin(new ExternalLoginRequest()
        {
            Provider = model.Provider,
            Identity = identityObject,
            ReturnUrl = model.ReturnUrl,
        });
    }

    private async Task<IActionResult> ContinueExternalLogin(ExternalLoginRequest request)
    {
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
}
