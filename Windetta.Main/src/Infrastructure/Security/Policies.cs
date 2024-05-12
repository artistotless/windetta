using Microsoft.AspNetCore.Authorization;

namespace Windetta.Main.Infrastructure.Security;

internal static class Policies
{
    internal static void Configure(AuthorizationOptions options)
    {
        options.AddPolicy("NeedBeUser", p =>
        {
            p.RequireAuthenticatedUser();
            p.RequireClaim(JwtClaimTypes.Subject);
        });

        options.AddPolicy("NeedRealtimeScope", p =>
        {
            p.RequireAuthenticatedUser();
            p.RequireClaim(JwtClaimTypes.Subject);
            p.RequireClaim(JwtClaimTypes.Scope, "realtime");
        });
    }
}
