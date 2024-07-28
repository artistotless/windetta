using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Authentication;
using Windetta.Common.Types;
using Windetta.Contracts.Events;
using Windetta.Contracts.Responses;
using Windetta.Main.Core.Matches;

namespace Windetta.Main.Web.Api;

public static class OngoingMatchesEndpoints
{
    public static void UseOngoingMatchesEndpoints(this WebApplication web)
    {
        // Get all ongoing matches IDs
        web.MapGet("api/matches/ongoing", async (
            [FromServices] IOngoingMatches matches) =>
        {
            var ids = await matches.GetAllIdsAsync();
            var response = new BaseResponse<IEnumerable<Guid>>(ids);

            return Results.Ok(response);
        });

        // Get ongoing match ID by playerID
        web.MapGet("api/players/{playerId:Guid}/matches/ongoing", async (
            [FromRoute] Guid playerId,
            [FromServices] IOngoingMatches matches,
            [FromServices] IUserIdProvider userIdProvider) =>
        {
            var matchId = await matches.GetMatchIdOfPlayerAsync(playerId);
            var response = new BaseResponse<Guid>(matchId);

            return Results.Ok(response);
        });

        // Get ongoing match data by ID
        web.MapGet("api/matches/ongoing/{matchId:Guid}", async (
            [FromRoute] Guid matchId,
            [FromServices] IRequestClient<IMatchInfoRequested> client) =>
        {
            var matchInfoResponse = await client.GetResponse<MatchInfoResponse>(new
            {
                CorrelationId = matchId
            });

            if (matchInfoResponse.Message is null
            || !matchInfoResponse.Message.Success)
                return Results.NotFound();

            var gameId = matchInfoResponse.Message.GameId;
            var players = matchInfoResponse.Message.Players;

            var match = new OngoingMatch(matchId, gameId, players)
            {
                Bet = matchInfoResponse.Message.Bet,
                Created = matchInfoResponse.Message.Created,
                GameServerEndpoint = matchInfoResponse.Message.GameServerEndpoint,
            };

            var response = new BaseResponse<OngoingMatch>(match);

            return Results.Ok(response);
        });
    }
}