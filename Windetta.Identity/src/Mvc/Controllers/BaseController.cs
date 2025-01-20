using IdentityModel;
using Microsoft.AspNetCore.Mvc;

namespace Windetta.Identity.Controllers;

public class BaseController : Controller
{
    protected string? UserId => User?.FindFirst(JwtClaimTypes.Subject)?.Value;

    protected IActionResult NoContent(Action action)
    {
        action?.Invoke();

        return NoContent();
    }

    protected IActionResult Single<T>(T model)
    {
        if (model is null)
            return NotFound();
        else
            return Ok(model);
    }
}
