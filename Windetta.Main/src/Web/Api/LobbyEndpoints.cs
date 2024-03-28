using Microsoft.AspNetCore.Mvc;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;
using Windetta.Main.Core.Services;
using Windetta.Main.Web.Api.Dtos;

namespace Windetta.Main.Web.Api;

public static class LobbyEnpoints
{
    public static void UseLobbyEndpoints(this WebApplication web)
    {
        //TODO: uncomment RequireAuthorization()
        var group = web.MapGroup("api/lobbies")/*.RequireAuthorization()*/;

        // Get lobbies
        web.MapGet("api/lobbies", async ([FromServices] ILobbies lobbies) =>
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
