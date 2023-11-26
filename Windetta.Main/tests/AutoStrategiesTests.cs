using Moq;
using Windetta.Main.Games;
using Windetta.Main.MatchHub;
using Windetta.Main.Rooms;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests;

public class AutoStrategiesTests
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

        var storageMock = new Mock<IMatchHubs>();
        var interactor = new MatchHubsInteractor(storageMock.Object);

        IMatchHub hub = await interactor.Create(options);

        var member1 = new RoomMember(Guid.NewGuid());
        var member2 = new RoomMember(Guid.NewGuid());
        var room = hub.Rooms.First();

        bool autoStrategyWorkedOut = false;
        var tcs = new TaskCompletionSource<bool>();

        EventHandler callback = delegate (object? sender, EventArgs e)
        {
            tcs.SetResult(true);
            autoStrategyWorkedOut = true;
        };

        hub.Ready += callback;

        // act
        hub.Add(member1, room.Id);
        hub.Add(member2, room.Id);

        await tcs.Task;

        // assert
        autoStrategyWorkedOut.ShouldBeTrue();
    }

    [Fact]
    public async Task TestAutoDisposeStrategy()
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
            AutoDisposeStrategy = new DateDisposeStrategy(
                checkInterval: TimeSpan.FromSeconds(1))
        };

        var storageMock = new Mock<IMatchHubs>();
        var interactor = new MatchHubsInteractor(storageMock.Object);

        IMatchHub hub = await interactor.Create(options);

        bool autoStrategyWorkedOut = false;
        var tcs = new TaskCompletionSource<bool>();

        EventHandler callback = delegate (object? sender, EventArgs e)
        {
            tcs.SetResult(true);
            autoStrategyWorkedOut = true;
        };

        hub.Disposed += callback;

        // act
        await tcs.Task;

        // assert
        autoStrategyWorkedOut.ShouldBeTrue();
    }
}
