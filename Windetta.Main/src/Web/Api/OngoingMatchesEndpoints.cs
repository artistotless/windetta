using Microsoft.AspNetCore.Mvc;
using Windetta.Main.Core.Matches;
using Windetta.Main.Core.Services;

namespace Windetta.Main.Web.Api;

public static class OngoingMatchesEndpoints
{
    public static void UseOngoingMatchesEndpoints(this WebApplication web)
    {
        // Get current match information
        web.MapGet("api/ongoingMatches", async (
            [FromServices] IOngoingMatches matches,
            [FromServices] IUserIdProvider userIdProvider) =>
        {
            Guid userId = userIdProvider.UserId;

            return Results.Ok(await matches.GetAsync(userId));
        });
    }
}