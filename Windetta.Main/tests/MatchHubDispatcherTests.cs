using Windetta.Common.Testing;
using Windetta.Main.Games;
using Windetta.Main.MatchHub;
using Windetta.MainTests.Mocks;
using Xunit.Abstractions;

namespace Windetta.MainTests;

public class MatchHubDispatcherTests
{
    private readonly XUnitOutWrapper _output;
    public MatchHubDispatcherTests(ITestOutputHelper output)
    {
        _output = new XUnitOutWrapper(output);
    }

    [Fact]
    public async Task UpdateMatchHubState_ShouldCausePrintingIntoOutputChannel()
    {
        // arrange
        var config = new GameConfiguration()
        {
            MaxPlayers = 2,
        };

        var options = new MatchHubOptions()
        {
            GameConfiguration = config,
            Bet = new Bet(currencyId: 1, bet: 100)
        };

        var buffer = new Queue<string>();
        var interactor = new MatchHubsInteractor(new Mock<IMatchHubs>().Object);
        var outputChannel = new MatchHubDebugOutputChannel(_output, ref buffer);
        var dispatcher = new MatchHubsDispatcher(outputChannel, new Mock<IMatchHubs>().Object);
        var hub = await interactor.CreateAsync(options, Guid.NewGuid());
        var roomId = hub.Rooms.First().Id;

        // act
        dispatcher.AddToTracking(hub);
        await interactor.JoinMember(Guid.NewGuid(), hub, roomId);

        // assert
        var text = $"Hub updated: {hub.Id}";
        buffer.Dequeue().ShouldBe(text);
    }

    [Fact]
    public async Task DisposeMatchHub_ShouldCausePrintingIntoOutputChannel()
    {
        // arrange
        var config = new GameConfiguration()
        {
            MaxPlayers = 2,
        };

        var options = new MatchHubOptions()
        {
            GameConfiguration = config,
            Bet = new Bet(currencyId: 1, bet: 100)
        };

        var buffer = new Queue<string>();
        var storage = new MatchHubsInMemoryStorage();
        var interactor = new MatchHubsInteractor(storage);
        var outputChannel = new MatchHubDebugOutputChannel(_output, ref buffer);
        var dispatcher = new MatchHubsDispatcher(outputChannel, storage);
        var hub = await interactor.CreateAsync(options, Guid.NewGuid());

        // act
        dispatcher.AddToTracking(hub);
        await interactor.DeleteAsync(hub.Id);

        // assert
        var text = $"Hub deleted: {hub.Id}";
        storage.Count.ShouldBe(0);
        buffer.Dequeue().ShouldBe(text);
    }

    [Fact]
    public async Task ReadyMatchHub_ShouldCausePrintingIntoOutputChannel()
    {
        // arrange
        var config = new GameConfiguration()
        {
            MaxPlayers = 3,
        };

        var options = new MatchHubOptions()
        {
            GameConfiguration = config,
            Bet = new Bet(currencyId: 1, bet: 100),
            AutoReadyStrategy = new FullRoomsReadyStrategy(TimeSpan.FromSeconds(1)),
        };

        var buffer = new Queue<string>();
        var storage = new MatchHubsInMemoryStorage();
        var interactor = new MatchHubsInteractor(storage);
        var outputChannel = new MatchHubDebugOutputChannel(_output, ref buffer);
        var dispatcher = new MatchHubsDispatcher(outputChannel, storage);
        var hub = await interactor.CreateAsync(options, Guid.NewGuid());
        var roomId = hub.Rooms.First().Id;

        // act
        dispatcher.AddToTracking(hub);
        await interactor.JoinMember(Guid.NewGuid(), hub.Id, roomId);
        await interactor.JoinMember(Guid.NewGuid(), hub.Id, roomId);
        await Task.Delay(1_000);

        // assert
        var text = $"Hub ready: {hub.Id}";
        buffer.Dequeue(); // Hub update
        buffer.Dequeue(); // Hub update
        buffer.Dequeue().ShouldBe(text); // Hub ready
    }
}
