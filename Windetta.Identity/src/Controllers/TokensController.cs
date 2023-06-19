using Microsoft.AspNetCore.Mvc;

namespace Windetta.Identity.Controllers;

public class TokensController : BaseController
{
    public IActionResult Index()
    {
        return Ok();
    }
}
