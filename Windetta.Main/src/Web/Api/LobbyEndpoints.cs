using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Windetta.Common.Types;
using Windetta.Main.Core.Clients.Dtos;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;
using Windetta.Main.Infrastructure.Services;
using Windetta.Main.Infrastructure.SignalR;

using IUserIdProvider = Windetta.Common.Authentication.IUserIdProvider;

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
            var response = new BaseResponse<UserLobbyMapEntry>
            (result.HasValue ? result.Value : default);

            return Results.Ok(response);
        });

        // Get lobbies
        group.MapGet("/", async ([FromServices] ILobbies lobbies) =>
        {
            var result = await lobbies.GetAllAsync();
            var response = new BaseResponse<IEnumerable<LobbyDto>>(result);

            return Results.Ok(response);
        });

        // Create lobby
        group.MapPost("/", async (
            [FromBody] CreateLobbyRequestDto body,
            [FromServices] IHubContext<MainHub> hub,
            [FromServices] IUserIdProvider userIdProvider,
            [FromServices] LobbyObserver observer,
            [FromServices] LobbiesInteractor interactor) =>
        {
            var userId = userIdProvider.UserId;
            var createRequest = new CreateLobbyDto()
            {
                Bet = body.Bet,
                InitiatorId = userId,
                GameId = body.GameId,
                Private = body.Private,
                Properties = body.Properties,
                JoinFilters = body.JoinFilters,
                AutoDisposeStrategy = body.AutoDisposeStrategy,
                AutoReadyStrategy = body.AutoReadyStrategy,
            };

            var lobby = await interactor.CreateAsync(createRequest);
            observer.AddToTracking(lobby);

            await hub.Clients.User(userId.ToString()).SendToMirrorAsync(new
            {
                method = MainHub.Methods.SubscribeOnLobbyFlow,
                data = lobby.Id
            });

            var response = new BaseResponse<ILobby>(lobby);

            return Results.Ok(response);
        });

        // Join room
        group.MapPost("/{lobbyId:guid}/rooms/{roomIndex:int}", async (
            [FromRoute] Guid lobbyId,
            [FromRoute] ushort roomIndex,
            [FromServices] IHubContext<MainHub> hub,
            [FromServices] IUserIdProvider userIdProvider,
            [FromServices] LobbiesInteractor interactor) =>
        {
            var userId = userIdProvider.UserId;

            await interactor.JoinMemberAsync
            (userId, lobbyId, roomIndex);

            await hub.Clients.User(userId.ToString()).SendToMirrorAsync(new
            {
                method = MainHub.Methods.SubscribeOnLobbyFlow,
                data = lobbyId
            });

            return Results.NoContent();
        });

        // Leave room
        group.MapDelete("/{lobbyId:guid}/rooms/{roomIndex:int}", async (
            [FromRoute] Guid lobbyId,
            [FromRoute] ushort roomIndex,
            [FromServices] IHubContext<MainHub> hub,
            [FromServices] IUserIdProvider userIdProvider,
            [FromServices] LobbiesInteractor interactor) =>
        {
            var userId = userIdProvider.UserId;

            await interactor.LeaveMemberAsync
            (userId, lobbyId, roomIndex);

            await hub.Clients.User(userId.ToString()).SendToMirrorAsync(new
            {
                method = MainHub.Methods.UnsubscribeFromLobbyFlow,
                data = lobbyId
            });

            return Results.NoContent();
        });
    }
}
