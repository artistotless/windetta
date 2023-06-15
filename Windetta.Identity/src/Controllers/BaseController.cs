using Microsoft.AspNetCore.Mvc;

namespace Windetta.Identity.Controllers;

public class BaseController : ControllerBase
{
    protected Guid UserId
        => string.IsNullOrWhiteSpace(User?.Identity?.Name) ?
            Guid.Empty :
            Guid.Parse(User.Identity.Name);
}
