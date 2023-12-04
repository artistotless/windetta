using Microsoft.Extensions.DependencyInjection;
using Windetta.Main.MatchHubs;
using Windetta.MainTests.Mocks;
using Windetta.MainTests.Shared;
using FullRoomsReadyStrategy = Windetta.MainTests.Mocks.FullRoomsReadyStrategy;

namespace Windetta.MainTests;

public class AutoStrategiesTests
{
    [Fact]
    public async Task TestFullRoomsReadyStrategy()
    {
        // arrange
        var request = new CreateMatchHubRequest()
        {
            Bet = new Bet(1, 1000),
            AutoReadyStrategy = nameof(FullRoomsReadyStrategy),
            GameId = IdExamples.GameId,
            InitiatorId = IdExamples.UserId
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
            AutoDisposeStrategy = nameof(DateDisposeStrategy),
            GameId = IdExamples.GameId,
            InitiatorId = IdExamples.UserId
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
