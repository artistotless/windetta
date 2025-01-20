using Microsoft.AspNetCore.Mvc;

namespace Windetta.Web.Controllers;

public class MatchHubsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
