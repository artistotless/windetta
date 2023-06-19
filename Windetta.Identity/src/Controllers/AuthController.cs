using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Services;

namespace Windetta.Identity.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : BaseController
{
    private readonly IRequestDispatcher _dispatcher;

    public AuthController(IRequestDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Authenticate user using email & password
    /// </summary>
    /// <returns>JsonWebToken object</returns>
    [HttpPost]
    [Route("signin")]
    public async Task<IActionResult> SignIn([FromBody] Login command)
        => await _dispatcher.HandleAsync(command);

    /// <summary>
    /// Register new user using email & password
    /// </summary>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] Register command)
        => await _dispatcher.HandleAsync(command);

    /// <summary>
    /// Authenticate user using external auth provider
    /// </summary>
    [HttpGet]
    [Route("signin/{provider}")]
    public async Task SignInWithVk(string provider, string returnUrl)
        => await ChallengeAsync(provider.ToLower(), returnUrl);

    /// <summary>
    /// Handle the response recieved from external Oauth provider
    /// </summary>
    [HttpGet]
    [Route("signin/external/callback")]
    public async Task<IActionResult> ExternalSignInCallback(string returnUrl, string provider)
    {
        var result = await HttpContext.AuthenticateAsync(provider);

        // Authentication failed
        if (result is null || !result.Succeeded)
            return Unauthorized();

        var command = new ExternalLogin()
        {
            Provider = provider,
            Identity = (ClaimsIdentity)result.Principal.Identity,
            ReturnUrl = returnUrl
        };

        // Authentication passed.
        // Return authCode across api gateway to end cliend.
        return await _dispatcher.HandleAsync(command);
    }

    /// <summary>
    /// Redirect to external OAuth screen page
    /// </summary>
    private Task ChallengeAsync(string scheme, string returnUrl)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(ExternalSignInCallback),
            new { returnUrl = returnUrl, provider = scheme })
        };

        return HttpContext.ChallengeAsync(scheme, properties);
    }
}
