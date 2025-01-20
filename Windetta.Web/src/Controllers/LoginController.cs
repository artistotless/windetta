using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Windetta.Web.Controllers;

public class LoginController : Controller
{
    [HttpGet]
    [AllowAnonymous]
    [Route("[controller]")]
    public async Task<IActionResult> Index(string returnUrl)
    {
        var authResult = await HttpContext.AuthenticateAsync
            (CookieAuthenticationDefaults.AuthenticationScheme);

        if (authResult.Principal is not null && authResult.Succeeded)
            return Redirect(returnUrl);

        return Challenge(new AuthenticationProperties()
        {
            RedirectUri = returnUrl,
        });
    }
}
