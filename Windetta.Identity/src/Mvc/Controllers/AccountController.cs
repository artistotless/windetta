using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Windetta.Identity.Extensions;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Services;

namespace Windetta.Identity.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : BaseController
{
    private readonly IRequestDispatcher _dispatcher;

    public AccountController(IRequestDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Entry point into the login workflow
    /// </summary>
    [HttpGet]
    [Route("login")]
    public async Task<IActionResult> Login([FromQuery] StartLoginFlowRequest request)
        => View(await _dispatcher.HandleAsync(request));

    /// <summary>
    /// Show register page for registering new user
    /// </summary>
    [HttpGet]
    [Route("register")]
    public IActionResult Register() => View();

    [HttpPost]
    [Route("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromForm] LocalLoginRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        try
        {
            var localLoginResponse = await _dispatcher.HandleAsync(request);

            return BuildRedirectResult(localLoginResponse.Context, request.ReturnUrl);

        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        var loginViewModel = await _dispatcher.HandleAsync(new StartLoginFlowRequest()
        {
            ReturnUrl = request.ReturnUrl,
        });

        loginViewModel.Username = request.Username;
        loginViewModel.RememberLogin = request.RememberLogin;

        return View(loginViewModel);
    }

    /// <summary>
    /// Register new user using email & password
    /// </summary>
    [HttpPost]
    [Route("register")]
    public IActionResult Register([FromBody] LocalRegisterRequest request)
        => NoContent(async () => await _dispatcher.HandleAsync(request));

    /// <summary>
    /// Show logout page
    /// </summary>
    [HttpGet]
    [Route("logout")]
    public async Task<IActionResult> Logout([FromQuery] Messages.Requests.LogoutRequest request)
    {
        var loggedOutResponse = await _dispatcher.HandleAsync(request);

        await HttpContext.SignOutAsync();

        if (loggedOutResponse.IsLocalLogout)
            return Redirect("~/");

        return View(loggedOutResponse);
    }

    #region private helpers

    private IActionResult BuildRedirectResult(AuthorizationRequest context, string returnUrl)
    {
        if (context is not null)
        {
            if (context.IsNativeClient())
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage("Redirect", returnUrl);

            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
            return Redirect(returnUrl);
        }

        // request for a local page
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        if (string.IsNullOrEmpty(returnUrl))
            return Redirect("~/");

        // user might have clicked on a malicious link - should be logged
        throw new Exception("invalid return URL");
    }
    #endregion
}
