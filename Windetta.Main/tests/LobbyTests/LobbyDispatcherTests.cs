using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Testing;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;
using Windetta.MainTests.Mocks;
using Xunit.Abstractions;

namespace Windetta.MainTests.LobbyTest;

public class LobbyDispatcherTests
{
    private readonly XUnitOutWrapper _output;
    public LobbyDispatcherTests(ITestOutputHelper output)
    {
        _output = new XUnitOutWrapper(output);
    }

    [Fact]
    public async Task UpdateLobbyState_ShouldCausePrintingIntoOutputChannel()
    {
        // arrange
        var request = new CreateLobbyDto()
        {
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            Bet = new Bet(currencyId: 1, bet: 100),
        };

        var buffer = new Queue<string>();
        var interactor = SharedServiceProvider.GetInstance()
            .GetRequiredService<LobbiesInteractor>();
        var outputChannel = new LobbyDebugOutput(_output, ref buffer);
        var dispatcher = new LobbyObserver(new Mock<ILobbies>().Object, outputChannel, null);
        var lobby = await interactor.CreateAsync(request);

        // act
        dispatcher.AddToTracking(lobby);
        await interactor.LeaveMemberAsync(request.InitiatorId, lobby.Id, roomIndex: 0);
        await interactor.JoinMemberAsync(request.InitiatorId, lobby.Id, roomIndex: 0);

        // assert
        var text = $"Lobby updated: {lobby.Id}";
        buffer.Dequeue().ShouldBe(text);
    }

    [Fact]
    public async Task DisposeLobby_ShouldCausePrintingIntoOutputChannel()
    {
        // arrange
        var provider = SharedServiceProvider.GetInstance();
        var request = new CreateLobbyDto()
        {
            Bet = new Bet(1, 1000),
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId
        };

        var buffer = new Queue<string>();
        var storage = provider.GetRequiredService<ILobbies>();
        var interactor = provider.GetRequiredService<LobbiesInteractor>();
        var outputChannel = new LobbyDebugOutput(_output, ref buffer);
        var dispatcher = new LobbyObserver(storage, outputChannel, null);
        var lobby = await interactor.CreateAsync(request);

        // act
        dispatcher.AddToTracking(lobby);
        await interactor.DeleteAsync(lobby.Id);

        // assert
        var text = $"Lobby deleted: {lobby.Id}";
        (await storage.GetAllAsync()).Count().ShouldBe(0);

        buffer.Dequeue(); // Lobby added
        buffer.Dequeue()  // Lobby deleted
            .ShouldBe(text);
    }

    [Fact]
    public async Task ReadyLobby_ShouldCausePrintingIntoOutputChannel()
    {
        // arrange
        var gameCfg = new GameConfiguration()
        {
            MaxPlayersInTeam = 3,
        };

        var sc = new[] { new SupportedCurrency(1, 1, 100000) };

        var provider = SharedServiceProvider.GetInstance((services) =>
        {
            services.AddSingleton((p) => new GamesRepositoryMock(gameCfg, sc).Mock.Object);
        });

        var request = new CreateLobbyDto()
        {
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            Bet = new Bet(currencyId: 1, bet: 100),
            AutoReadyStrategy = new PluginSetDto(nameof(FullRoomsReadyStrategy)),
        };

        var buffer = new Queue<string>();
        var storage = provider.GetRequiredService<ILobbies>();
        var interactor = provider.GetRequiredService<LobbiesInteractor>();
        var outputChannel = new LobbyDebugOutput(_output, ref buffer);
        var dispatcher = new LobbyObserver(storage, outputChannel, null);
        var lobby = await interactor.CreateAsync(request);

        // act
        dispatcher.AddToTracking(lobby);
        await interactor.JoinMemberAsync(Guid.NewGuid(), lobby.Id, roomIndex: 0);
        await interactor.JoinMemberAsync(Guid.NewGuid(), lobby.Id, roomIndex: 0);

        while (buffer.Count < 3 && new CancellationTokenSource
            (TimeSpan.FromSeconds(6)).IsCancellationRequested == false)
        {
            await Task.Delay(300);
        }

        // assert
        var text = $"Lobby ready: {lobby.Id}";
        buffer.Dequeue(); // Lobby added
        buffer.Dequeue(); // Lobby update
        buffer.Dequeue()  // Lobby ready
            .ShouldBe(text);
    }
}
