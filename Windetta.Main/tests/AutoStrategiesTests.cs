using Windetta.Main.Games;
using Windetta.Main.MatchHub;
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

        var interactor = new MatchHubsInteractor(new Mock<IMatchHubs>().Object);

        IMatchHub hub = await interactor.CreateAsync(options);

        var member1Id = Guid.NewGuid();
        var member2Id = Guid.NewGuid();
        var roomId = hub.Rooms.First().Id;

        bool autoStrategyWorkedOut = false;
        var tcs = new TaskCompletionSource<bool>();

        EventHandler callback = delegate (object? sender, EventArgs e)
        {
            tcs.SetResult(true);
            autoStrategyWorkedOut = true;
        };

        hub.Ready += callback;

        // act
        interactor.JoinMember(member1Id, hub, roomId);
        interactor.JoinMember(member2Id, hub, roomId);

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

        var interactor = new MatchHubsInteractor(new Mock<IMatchHubs>().Object);

        IMatchHub hub = await interactor.CreateAsync(options);

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
