using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Authentication;
using Windetta.Identity.Messages.Comands;
using Windetta.Identity.Services;

namespace Windetta.Identity.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : BaseController
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    /// <summary>
    /// Authenticate user using email & password
    /// </summary>
    /// <returns>JsonWebToken object</returns>
    [HttpPost]
    [Route("signin")]
    public async Task<ActionResult<JsonWebToken>> SignIn([FromBody] Login command)
    {
        await Task.Delay(100);

        return Ok(new JsonWebToken());
    }

    /// <summary>
    /// Register new user using email & password
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] Register command)
    {
        await _identityService.RegisterAsync(command);

        return NoContent();
    }

    /// <summary>
    /// Authenticate user using external auth provider 'vk.com'
    /// </summary>
    [HttpGet]
    [Route("signin/vk")]
    public async Task SignInWithVk(string returnUrl)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(SignInWithVkCallback), new { returnUrl })
        };

        await HttpContext.ChallengeAsync("Vkontakte", properties);
    }

    /// <summary>
    /// Handle the response recieved from Vk OAuth
    /// </summary>
    [HttpGet]
    [Route("signin/vk/callback")]
    public async Task<IActionResult> SignInWithVkCallback(string returnUrl)
    {
        var result = await HttpContext.AuthenticateAsync("Vkontakte");

        // Authentication failed
        if (result is null || !result.Succeeded)
            return Unauthorized();

        var jwt = await _identityService.ExternalLoginAsync(new ExternalLogin()
        {
            Provider = "Vkontakte",
            UniqueIdentifier = result.Principal.Identity!.Name!,
            UserId = UserId,
        });

        return Redirect(returnUrl);
    }

    /// <summary>
    /// Authenticate user using external auth provider 'google.com'
    /// </summary>
    [HttpGet]
    [Route("signin/google")]
    public async Task SignInWithGoogle()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(SignInWithGoogleCallback))
        };

        await HttpContext.ChallengeAsync("Google", properties);
    }

    /// <summary>
    /// Handle the response recieved from Google OAuth
    /// </summary>
    [HttpGet]
    [Route("signin/google/callback")]
    public async Task<IActionResult> SignInWithGoogleCallback()
    {
        var result = await HttpContext.AuthenticateAsync("Google");
        if (result?.Succeeded == true)
        {
            // Аутентификация прошла успешно
            // Вы можете получить информацию об аутентифицированном пользователе через `result.Principal`
            return Ok();
        }

        // Аутентификация не удалась или отсутствует
        return Unauthorized();
    }
}
