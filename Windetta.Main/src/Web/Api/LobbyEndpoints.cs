using Microsoft.AspNetCore.Mvc;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Services;

namespace Windetta.Main.Web.Api
{
    public static class LobbyEnpoints
    {
        public static void UseLobbyEndpoints(this WebApplication web)
        {
            // Get lobbies
            web.MapGet("api/lobbies", async ([FromServices] ILobbies lobbies) =>
            {
                return Results.Ok(await lobbies.GetAllAsync());
            });

            // Create lobby
            web.MapPost("api/lobbies", async (
                [FromBody] CreateLobbyRequest body,
                [FromServices] LobbiesInteractor interactor) =>
            {
                var lobby = await interactor.CreateAsync(body);

                return Results.Ok(lobby);
            });

            // Join room
            web.MapPost("api/lobbies/{id:guid}/rooms/{roomIndex:int}", async (
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
            web.MapDelete("api/lobbies/{id:guid}/rooms/{roomIndex:int}", async (
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
}
