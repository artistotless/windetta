using Microsoft.AspNetCore.Mvc;

namespace Windetta.Identity.Controllers;

public class RolesController : BaseController
{
    public IActionResult Index()
    {
        return Ok();
    }
}
