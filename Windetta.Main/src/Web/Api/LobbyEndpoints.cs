using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Authentication;
using Windetta.Main.Core.Clients.Dtos;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;
using Windetta.Main.Infrastructure.Services;

namespace Windetta.Main.Web.Api;

public static class LobbyEndpoints
{
    public static void UseLobbyEndpoints(this WebApplication web)
    {
        var group = web.MapGroup("api/lobbies");

        // Get user's lobby information
        web.MapGet("api/users/{userId:guid}/lobby", (
            [FromRoute] Guid userId,
            [FromServices] IUserLobbyMaps maps) =>
        {
            var result = maps.Get(userId);
            return result.HasValue ? Results.Ok(result) : Results.NotFound();
        });

        // Get lobbies
        group.MapGet("/", async ([FromServices] ILobbies lobbies) =>
        {
            return Results.Ok(await lobbies.GetAllAsync());
        });

        // Create lobby
        group.MapPost("/", async (
            [FromBody] CreateLobbyRequestDto body,
            [FromServices] IUserIdProvider userIdProvider,
            [FromServices] LobbyObserver observer,
            [FromServices] LobbiesInteractor interactor) =>
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

            var lobby = await interactor.CreateAsync(createRequest);

            observer.AddToTracking(lobby);

            return Results.Ok(lobby);
        });

        // Join room
        group.MapPost("/{lobbyId:guid}/rooms/{roomIndex:int}", async (
            [FromRoute] Guid lobbyId,
            [FromRoute] ushort roomIndex,
            [FromServices] IUserIdProvider userIdProvider,
            [FromServices] LobbiesInteractor interactor) =>
        {
            await interactor.JoinMemberAsync
            (userIdProvider.UserId, lobbyId, roomIndex);

            return Results.NoContent();
        });

        // Leave room
        group.MapDelete("/{lobbyId:guid}/rooms/{roomIndex:int}", async (
            [FromRoute] Guid lobbyId,
            [FromRoute] ushort roomIndex,
            [FromServices] IUserIdProvider userIdProvider,
            [FromServices] LobbiesInteractor interactor) =>
        {
            await interactor.LeaveMemberAsync
            (userIdProvider.UserId, lobbyId, roomIndex);

            return Results.NoContent();
        });
    }
}
