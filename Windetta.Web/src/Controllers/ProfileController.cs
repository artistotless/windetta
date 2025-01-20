using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Windetta.Common.Types;
using Windetta.Web.Models;

namespace Windetta.Web.Controllers;

[ApiController]
public class ProfileController : ControllerBase
{
    [HttpGet]
    [Route("[controller]")]
    public async Task<ActionResult<Profile>> Index()
    {
        try
        {
            var authResult = await HttpContext.AuthenticateAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = authResult.Principal;

            if (principal is null || principal.Identity is null)
                return StatusCode(400);

            var response = new BaseResponse<Profile>(new Profile()
            {
                Id = Guid.TryParse(principal.FindFirstValue("sub"), out Guid id) ? id : Guid.Empty,
                DisplayName = principal.FindFirstValue("nickname"),
                Email = principal.FindFirstValue("email"),
                Username = principal.Identity?.Name
            });

            return Ok(response);
        }
        catch
        {
            return StatusCode(500);
        }
    }
}
