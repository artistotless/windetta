using IdentityServer4.Models;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Windetta.Contracts.Events;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Extensions;
using Windetta.Identity.Infrastructure.Exceptions;
using Windetta.Identity.Messages.Events;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Models;
using Windetta.Identity.Services;

namespace Windetta.Identity.Controllers;


[Route("[controller]")]
public class AccountController : BaseController
{
    private readonly IRequestDispatcher _dispatcher;
    private readonly UserManager<User> _userManager;
    private readonly IPublishEndpoint _bus;
    private readonly IBus _bus2;


    public AccountController(IRequestDispatcher dispatcher, UserManager<User> userManager, IPublishEndpoint bus, IBus bus2)
    {
        _dispatcher = dispatcher;
        _userManager = userManager;
        _bus = bus;
        _bus2 = bus2;
    }

    [HttpGet]
    [Route("event")]
    public async Task<IActionResult> Event()
    {
        return NoContent(async () => await _bus.Publish<UserCreated>(new
        {
            Email = "test@email.com",
            Id = Guid.NewGuid(),
            Role = "user",
            UserName = "userName",
        }));
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([FromForm] RegisterInputModel model)
    {
        await ValidatePassword(model.Password);

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _dispatcher.HandleAsync(new LocalRegisterRequest()
            {
                Email = model.Email,
                Password = model.Password,
                UserName = model.Username
            });
        }
        catch (IdentityException e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
            return View(model);
        }

        return View("Registered", model);
    }

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
    private async Task ValidatePassword(string password)
    {
        var passValidator = _userManager.PasswordValidators.First();

        var passValidation = await passValidator.ValidateAsync(_userManager, null, password);
        if (passValidation.Succeeded)
            return;

        foreach (var error in passValidation.Errors)
            ModelState.AddModelError("Password", error.Description);
    }

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
