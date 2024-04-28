using MassTransit;
using Windetta.Contracts;
using Windetta.Contracts.Events;
using Windetta.Main.Core.Lobbies;

namespace Windetta.Main.Web.Api
{
    public static class TestEndpoints
    {
        public static void UserTestEndpoints( this WebApplication web)
        {
            web.MapPost("/lobby-ready", async
                (IPublishEndpoint publisher,
                LobbiesInteractor interactor) =>
            {
                var matchId = Guid.Parse("195da05a-d3ee-4d8b-917c-a77cf7afa906");
                var gameId = Guid.Parse("accea9d1-7f70-40e2-8a8d-a90d3a79842b");
                var player1Id = Guid.Parse("08dbc8b3-4170-4972-8728-c4ff931915f1");
                var player2Id = Guid.Parse("08dbc8b3-bf87-469c-8649-74c8d7b14255");

                var lobby = await interactor.CreateAsync(new()
                {
                    Bet = new FundsInfo(1, 100),
                    GameId = gameId,
                    InitiatorId = player1Id
                });

                await interactor.JoinMemberAsync(player2Id, lobby.Id, lobby.Rooms.First().Index);

                await publisher.Publish<ILobbyReady>(new
                {
                    TimeStamp = DateTimeOffset.UtcNow,
                    CorrelationId = lobby.Id,
                });

                return Results.Ok(lobby);
            });
        }
    }
}
