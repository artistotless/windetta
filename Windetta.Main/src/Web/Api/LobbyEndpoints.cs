using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Authentication;
using Windetta.Main.Core.Clients;
using Windetta.Main.Core.Clients.Dtos;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;
using Windetta.Main.Infrastructure.Services;

namespace Windetta.Main.Web.Api;

public static class LobbyEndpoints
{
    public static void UseLobbyEndpoints(this WebApplication web)
    {
        // Get lobbies
        web.MapGet("api/lobbies", async ([FromServices] ILobbies lobbies) =>
        {
            return Results.Ok(await lobbies.GetAllAsync());
        });

        var group = web.MapGroup("api/lobbies");

        // Create lobby
        group.MapPost("/", async (
            [FromBody] CreateLobbyRequestDto body,
            [FromServices] IUserIdProvider userIdProvider,
            [FromServices] LobbyObserver observer,
            [FromServices] ILobbyEndpointsClient client) =>
        {
            var createRequest = new CreateLobbyDto()
            {
                Bet = body.Bet,
                InitiatorId = userIdProvider.UserId,
                GameId = body.GameId,
                Private = body.Private,
                Properties = body.Properties,
                JoinFilters = body.JoinFilters,
                AutoDisposeStrategy = body.AutoDisposeStrategy,
                AutoReadyStrategy = body.AutoReadyStrategy,
            };

            var lobby = await client.CreateAsync(createRequest);

            observer.AddToTracking(lobby);

            return Results.Ok(lobby);
        });

        // Join room
        group.MapPost("/{lobbyId:guid}/rooms/{roomIndex:int}", async (
            [FromRoute] Guid lobbyId,
            [FromRoute] ushort roomIndex,
            [FromServices] IUserIdProvider userIdProvider,
            [FromServices] ILobbyEndpointsClient client) =>
        {
            await client.JoinMemberAsync
            (userIdProvider.UserId, lobbyId, roomIndex);

            return Results.NoContent();
        });

        // Leave room
        group.MapDelete("/{lobbyId:guid}/rooms/{roomIndex:int}", async (
            [FromRoute] Guid lobbyId,
            [FromRoute] ushort roomIndex,
            [FromServices] IUserIdProvider userIdProvider,
            [FromServices] ILobbyEndpointsClient client) =>
        {
            await client.LeaveMemberAsync
            (userIdProvider.UserId, lobbyId, roomIndex);

            return Results.NoContent();
        });
    }
}
