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
    public async Task<IActionResult> SignIn([FromBody] LoginRequest command)
        => Single(await _dispatcher.HandleAsync(command));

    /// <summary>
    /// Register new user using email & password
    /// </summary>
    [HttpPost]
    [Route("register")]
    public IActionResult Register([FromBody] RegisterRequest command)
        => NoContent(async () => await _dispatcher.HandleAsync(command));

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

        var identity = authentication.Principal.Identity;
        var command = new ExternalLoginRequest()
        {
            Provider = provider,
            Identity = identity as ClaimsIdentity,
            ReturnUrl = returnUrl
        };

        // Authentication passed.
        // Return authCode across api gateway to end cliend.
        var redirectUrl = await _dispatcher.HandleAsync(command);

        return new RedirectResult(redirectUrl);
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
