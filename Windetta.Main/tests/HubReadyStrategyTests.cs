using Windetta.Main.Games;
using Windetta.Main.MatchHub;
using Windetta.Main.MatchHub.Strategies;
using Windetta.Main.Rooms;

namespace Windetta.MainTests;

public class HubReadyStrategyTests
{
    [Fact]
    public async Task TestFullRoomsReadyStrategy()
    {
        // arrange
        var config = new GameConfiguration()
        {
            MinPlayers = 1,
            MaxPlayers = 2,
        };

        var options = new MatchHubOptions()
        {
            GameConfiguration = config,
            Bet = new Bet(1, 1000),
            Private = false,
            AutoReadyStrategy = new FullRoomsReadyStrategy(
                checkInterval: TimeSpan.FromSeconds(1))
        };

        IMatchHub hub = new MatchHub(options);

        var member1 = new RoomMember(Guid.NewGuid());
        var member2 = new RoomMember(Guid.NewGuid());
        var room = hub.Rooms.First();

        bool autoReadyStrategyWorkedOut = false;
        var tcs = new TaskCompletionSource<bool>();

        EventHandler callback = delegate (object? sender, EventArgs e)
        {
            tcs.SetResult(true);
            autoReadyStrategyWorkedOut = true;
        };

        hub.Ready += callback;

        // act
        hub.Add(member1, room.Id);
        hub.Add(member2, room.Id);

        await tcs.Task;

        // assert
        autoReadyStrategyWorkedOut.ShouldBeTrue();
    }
}
