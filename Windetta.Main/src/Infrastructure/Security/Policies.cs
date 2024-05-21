using Microsoft.AspNetCore.Authorization;
using Windetta.Common.Types;

namespace Windetta.Main.Infrastructure.Security;

internal static class Policies
{
    internal static void Configure(AuthorizationOptions options)
    {
        options.AddPolicy("RequireBeingUser", p =>
        {
            p.RequireAuthenticatedUser();
            p.RequireClaim(JwtClaimTypes.Subject);
        });

        options.AddPolicy("RequireRealtimeScope", p =>
        {
            p.RequireAuthenticatedUser();
            p.RequireClaim(JwtClaimTypes.Subject);
            p.RequireClaim(JwtClaimTypes.Scope, "realtime");
        });
    }
}
