using Microsoft.AspNetCore.Mvc;

namespace Windetta.Identity.Controllers;

public class BaseController : ControllerBase
{
    protected Guid UserId
        => string.IsNullOrWhiteSpace(User?.Identity?.Name) ?
            Guid.Empty :
            Guid.Parse(User.Identity.Name);

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
