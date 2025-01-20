using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Windetta.Web.Controllers;

public class LogoutController : Controller
{
    [HttpGet]
    [Route("[controller]")]
    public async Task<IActionResult> Index(string returnUrl)
    {
        await HttpContext.SignOutAsync
            (CookieAuthenticationDefaults.AuthenticationScheme);

        return Redirect(returnUrl);
    }
}
