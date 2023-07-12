using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Windetta.Common.IdentityServer;
using Windetta.Identity.Models;

namespace IdentityServerHost.Quickstart.UI
{
    //[SecurityHeaders]
    //[AllowAnonymous]
    //[Authorize(Roles = Roles.ADMIN)]
    [Authorize(AuthenticationSchemes = BearerCookies.Scheme)]
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;
        private readonly IAuthenticationSchemeProvider _a;

        public HomeController(IIdentityServerInteractionService interaction, IWebHostEnvironment environment, IAuthenticationSchemeProvider a)
        {
            _interaction = interaction;
            _environment = environment;
            _a = a;
        }

        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            var u = User;

            if (_environment.IsDevelopment())
            {
                // only show in development
                return View();
            }

            return NotFound();
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;

                if (!_environment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = null;
                }
            }

            return View("Error", vm);
        }
    }
}