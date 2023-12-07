using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Testing;
using Windetta.Main.Core.MatchHubs.Dtos;
using Windetta.MainTests.Mocks;
using Xunit.Abstractions;

namespace Windetta.MainTests.MatchHub;

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
        var request = new CreateMatchHubRequest()
        {
            GameId = IdExamples.GameId,
            InitiatorId = IdExamples.UserId,
            Bet = new Bet(currencyId: 1, bet: 100),
        };

        var buffer = new Queue<string>();
        var interactor = SharedServiceProvider.GetInstance()
            .GetRequiredService<MatchHubsInteractor>();
        var outputChannel = new MatchHubDebugOutput(_output, ref buffer);
        var dispatcher = new MatchHubObserver(outputChannel, new Mock<IMatchHubs>().Object);
        var hub = await interactor.CreateAsync(request);
        var roomId = hub.Rooms.First().Id;

        // act
        dispatcher.AddToTracking(hub);
        await interactor.LeaveMember(request.InitiatorId, hub.Id);
        await interactor.JoinMember(request.InitiatorId, hub.Id, roomId);

        // assert
        var text = $"Hub updated: {hub.Id}";
        buffer.Dequeue().ShouldBe(text);
    }

    [Fact]
    public async Task DisposeMatchHub_ShouldCausePrintingIntoOutputChannel()
    {
        // arrange
        var provider = SharedServiceProvider.GetInstance();
        var request = new CreateMatchHubRequest()
        {
            Bet = new Bet(1, 1000),
            GameId = IdExamples.GameId,
            InitiatorId = IdExamples.UserId
        };

        var buffer = new Queue<string>();
        var storage = provider.GetRequiredService<IMatchHubs>();
        var interactor = provider.GetRequiredService<MatchHubsInteractor>();
        var outputChannel = new MatchHubDebugOutput(_output, ref buffer);
        var dispatcher = new MatchHubObserver(outputChannel, storage);
        var hub = await interactor.CreateAsync(request);

        // act
        dispatcher.AddToTracking(hub);
        await interactor.DeleteAsync(hub.Id);

        // assert
        var text = $"Hub deleted: {hub.Id}";
        (await storage.GetAllAsync()).Count().ShouldBe(0);
        buffer.Dequeue().ShouldBe(text);
    }

    [Fact]
    public async Task ReadyMatchHub_ShouldCausePrintingIntoOutputChannel()
    {
        // arrange
        var gameCfg = new GameConfiguration()
        {
            MaxPlayers = 3,
        };

        var sc = new[] { new SupportedCurrency(1, 1, 100000) };

        var provider = SharedServiceProvider.GetInstance((services) =>
        {
            services.AddSingleton((p) => new GamesRepositoryMock(gameCfg, sc).Mock.Object);
        });

        var request = new CreateMatchHubRequest()
        {
            GameId = IdExamples.GameId,
            InitiatorId = IdExamples.UserId,
            Bet = new Bet(currencyId: 1, bet: 100),
            AutoReadyStrategy = new PluginSetDto(nameof(FullRoomsReadyStrategy)),
        };

        var buffer = new Queue<string>();
        var storage = provider.GetRequiredService<IMatchHubs>();
        var interactor = provider.GetRequiredService<MatchHubsInteractor>();
        var outputChannel = new MatchHubDebugOutput(_output, ref buffer);
        var dispatcher = new MatchHubObserver(outputChannel, storage);
        var hub = await interactor.CreateAsync(request);
        var roomId = hub.Rooms.First().Id;

        // act
        dispatcher.AddToTracking(hub);
        await interactor.JoinMember(Guid.NewGuid(), hub.Id, roomId);
        await interactor.JoinMember(Guid.NewGuid(), hub.Id, roomId);

        while (buffer.Count < 3 && new CancellationTokenSource
            (TimeSpan.FromSeconds(6)).IsCancellationRequested == false)
        {
            await Task.Delay(100);
        }

        // assert
        var text = $"Hub ready: {hub.Id}";
        buffer.Dequeue(); // Hub update
        buffer.Dequeue(); // Hub update
        buffer.Dequeue().ShouldBe(text); // Hub ready
    }
}
