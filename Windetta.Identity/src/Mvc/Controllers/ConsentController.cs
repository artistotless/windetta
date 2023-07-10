using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Windetta.Identity.Attributes;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Services;

namespace Windetta.Identity.Controllers;

[Authorize]
[SecurityHeaders]
public class ConsentController : BaseController
{
    private readonly IRequestDispatcher _dispatcher;

    public ConsentController(IRequestDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Entry point into the login workflow
    /// </summary>
    [HttpGet]
    [Route("consent")]
    public async Task<IActionResult> Index([FromQuery] ShowConsentScreenRequest request)
        => View(await _dispatcher.HandleAsync(request));
}
