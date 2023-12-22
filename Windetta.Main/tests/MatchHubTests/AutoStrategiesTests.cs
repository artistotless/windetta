using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Testing;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Core.MatchHubs.Dtos;
using Windetta.MainTests.Mocks;
using FullRoomsReadyStrategy = Windetta.MainTests.Mocks.FullRoomsReadyStrategy;

namespace Windetta.MainTests.MatchHubTest;

public class AutoStrategiesTests
{
    [Fact]
    public async Task TestFullRoomsReadyStrategy()
    {
        // arrange
        var request = new CreateMatchHubRequest()
        {
            Bet = new Bet(1, 1000),
            AutoReadyStrategy = new PluginSetDto(nameof(FullRoomsReadyStrategy)),
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId
        };

        var interactor = SharedServiceProvider.GetInstance()
            .GetRequiredService<MatchHubsInteractor>();

        IMatchHub hub = await interactor.CreateAsync(request);

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
        await interactor.JoinMember(member2Id, hub.Id, roomId);

        await tcs.Task;

        // assert
        autoStrategyWorkedOut.ShouldBeTrue();
    }

    [Fact]
    public async Task TestAutoDisposeStrategy()
    {
        // arrange
        var request = new CreateMatchHubRequest()
        {
            Bet = new Bet(1, 800),
            AutoDisposeStrategy = new PluginSetDto(nameof(DateDisposeStrategy)),
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId
        };

        var interactor = SharedServiceProvider.GetInstance()
            .GetRequiredService<MatchHubsInteractor>();

        IMatchHub hub = await interactor.CreateAsync(request);
        bool autoStrategyWorkedOut = false;
        var tcs = new TaskCompletionSource<bool>();

        EventHandler callback = delegate (object? sender, EventArgs e)
        {
            tcs.SetResult(true);
            autoStrategyWorkedOut = true;
        };


        hub.Disposed += callback;
        await interactor.LeaveMember(request.InitiatorId, hub.Id);

        // act
        await tcs.Task;

        // assert
        autoStrategyWorkedOut.ShouldBeTrue();
    }
}
