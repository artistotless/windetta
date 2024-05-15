using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Authentication;
using Windetta.Contracts.Events;
using Windetta.Contracts.Responses;
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
        web.MapGet("api/players/{playerId:Guid}/matches/ongoing", async (
            [FromRoute] Guid playerId,
            [FromServices] IOngoingMatches matches,
            [FromServices] IUserIdProvider userIdProvider) =>
        {
            return Results.Ok(await matches.GetAsync(playerId));
        });

        // Get current match information
        web.MapGet("api/matches/ongoing/{matchId:Guid}", async (
            [FromRoute] Guid matchId,
            [FromServices] IRequestClient<IMatchInfoRequested> client) =>
        {
            var response = await client.GetResponse<MatchInfoResponse>(new
            {
                CorrelationId = matchId
            });

            if (response.Message is null || !response.Message.Success)
                return Results.NotFound();

            return Results.Ok(new OngoingMatch()
            {
                Bet = response.Message.Bet,
                Created = response.Message.Created,
                GameId = response.Message.GameId,
                MatchId = response.Message.MatchId,
                Players = response.Message.Players,
                GameServerEndpoint = response.Message.GameServerEndpoint,
            });

        });
    }
}