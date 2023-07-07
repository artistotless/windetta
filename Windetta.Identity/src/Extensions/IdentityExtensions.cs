using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Windetta.Identity.Infrastructure.Exceptions;
using Windetta.Identity.Models;

namespace Windetta.Identity.Extensions;

public static class IdentityExtensions
{
    public static IdentityException FirstErrorAsException(this IEnumerable<IdentityError> errors)
    {
        var firstError = errors.First();

        return new IdentityException(firstError.Code, firstError.Description);
    }

    public static Claim? FindFirst(this IEnumerable<Claim> claims, string type)
        => claims.FirstOrDefault(x => x.Type.Equals(type));

    /// <summary>
    /// Checks if the redirect URI is for a native client.
    /// </summary>
    /// <returns></returns>
    public static bool IsNativeClient(this AuthorizationRequest context)
    {
        return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal)
           && !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
    }

    public static IActionResult LoadingPage(this Controller controller, string viewName, string redirectUri)
    {
        controller.HttpContext.Response.StatusCode = 200;
        controller.HttpContext.Response.Headers["Location"] = "";

        return controller.View(viewName, new RedirectViewModel { RedirectUrl = redirectUri });
    }
}
