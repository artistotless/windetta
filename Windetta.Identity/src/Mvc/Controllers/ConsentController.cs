using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Windetta.Identity.Attributes;
using Windetta.Identity.Extensions;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Models;
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

    [HttpPost]
    [Route("consent")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromForm] ConsentInputModel model)
    {
        var consentProcessResult = await _dispatcher.HandleAsync(new ProcessConsentRequest()
        {
            InputModel = model
        });

        if (consentProcessResult.IsRedirect)
        {
            if (consentProcessResult.IsNativeClient)
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage("Redirect", consentProcessResult.RedirectUri);
            }

            return Redirect(consentProcessResult.RedirectUri);
        }

        if (consentProcessResult.HasValidationError)
        {
            ModelState.AddModelError(string.Empty, consentProcessResult.ValidationError);

            var consentViewModel = await _dispatcher.HandleAsync(new ShowConsentScreenRequest()
            {
                Consent = model,
                ReturnUrl = model.ReturnUrl
            });

            consentProcessResult.ViewModel = consentViewModel;
        }

        if (consentProcessResult.ShowView)
            return View(consentProcessResult.ViewModel);

        return View("Error");
    }
}
