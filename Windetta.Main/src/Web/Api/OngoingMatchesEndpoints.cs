using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Authentication;
using Windetta.Main.Core.Clients;

namespace Windetta.Main.Web.Api;

public static class OngoingMatchesEndpoints
{
    public static void UseOngoingMatchesEndpoints(this WebApplication web)
    {
        // Get current match information
        web.MapGet("api/ongoingMatches", async (
            [FromServices] IOngoingMatchesEndpointsClient client,
            [FromServices] IUserIdProvider userIdProvider) =>
        {
            Guid userId = userIdProvider.UserId;

            return Results.Ok(await client.GetAsync(userId));
        });
    }
}