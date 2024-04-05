using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Authentication;
using Windetta.Main.Core.Matches;

namespace Windetta.Main.Web.Api;

public static class OngoingMatchesEndpoints
{
    public static void UseOngoingMatchesEndpoints(this WebApplication web)
    {
        // Get all current matches information
        web.MapGet("api/matches/ongoing", async (
            [FromServices] IOngoingMatches matches) =>
        {

            return Results.Ok(await matches.GetAllAsync());
        });

        // Get current match information
        web.MapGet("api/matches/ongoing/{playerId:Guid}", async (
            [FromServices] IOngoingMatches matches,
            [FromServices] IUserIdProvider userIdProvider) =>
        {
            Guid userId = userIdProvider.UserId;

            return Results.Ok(await matches.GetAsync(userId));
        });
    }
}