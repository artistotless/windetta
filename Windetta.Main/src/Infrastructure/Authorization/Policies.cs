using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Windetta.Main.Infrastructure.Authorization;

internal static class Policies
{
    internal static void Configure(AuthorizationOptions options)
    {
        options.AddPolicy("NeedBeUser", p =>
        {
            p.RequireAuthenticatedUser();
            p.RequireClaim(ClaimTypes.NameIdentifier);
        });

        options.AddPolicy("NeedRealtimeScope", p =>
        {
            p.RequireAuthenticatedUser();
            p.RequireClaim(ClaimTypes.NameIdentifier);
            p.RequireClaim("Scope", "realtime");
        });
    }
}