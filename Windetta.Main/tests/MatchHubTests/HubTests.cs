using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Testing;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.MatchHubs;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.MatchHubTest;
public class HubTests
{
    [Fact]
    public async Task ShouldCreatesRoomsAccordingGameConfig()
    {
        // arrange
        var gameCfg = new GameConfiguration()
        {
            MinPlayers = 1,
            MaxPlayers = (uint)Random.Shared.Next(10, 1000),
            MinTeams = 1,
            MaxTeams = (uint)Random.Shared.Next(1, 1000)
        };

        var sc = new[] { new SupportedCurrency(1, 1, 100000) };

        var provider = SharedServiceProvider.GetInstance((services) =>
        {
            services.AddSingleton((p) => new GamesRepositoryMock(gameCfg, sc).Mock.Object);
        });

        var request = new CreateMatchHubRequest()
        {
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            Bet = new Bet(currencyId: 1, bet: 100)
        };

        // act
        var interactor = provider.GetRequiredService<MatchHubsInteractor>();

        IMatchHub hub = await interactor.CreateAsync(request);

        // assert
        hub.Rooms.Count().ShouldBe((int)gameCfg.MaxTeams);
        hub.Rooms.First().MaxMembers.ShouldBe(gameCfg.MaxPlayers);
    }

    [Fact]
    public async void ShouldAddMemberToSelectedRoom()
    {
        // arrange
        var request = new CreateMatchHubRequest()
        {
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            Bet = new Bet(currencyId: 1, bet: 100)
        };

        var interactor = SharedServiceProvider.GetInstance()
            .GetRequiredService<MatchHubsInteractor>();

        var memberId = Guid.NewGuid();

        // act
        IMatchHub hub = await interactor.CreateAsync(request);
        var room = hub.Rooms.First();
        await interactor.JoinMemberAsync(memberId, hub.Id, room.Index);

        // assert
        room.Members.ShouldContain(x => x.Id == memberId);
    }

    [Fact]
    public async void JoinMemberShouldCauseUpdateEvent()
    {
        // arrange
        var request = new CreateMatchHubRequest()
        {
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            Bet = new Bet(currencyId: 1, bet: 100)
        };

        var interactor = SharedServiceProvider.GetInstance()
            .GetRequiredService<MatchHubsInteractor>();

        IMatchHub hub = await interactor.CreateAsync(request);
        var memberId = Guid.NewGuid();
        bool updateEventRaised = false;
        var tcs = new TaskCompletionSource<bool>();

        EventHandler callback = delegate (object? sender, EventArgs e)
        {
            tcs.SetResult(true);
            updateEventRaised = true;
        };

        hub.Updated += callback;

        // act
        await interactor.JoinMemberAsync(memberId, hub.Id, roomIndex: 0);

        // assert
        updateEventRaised.ShouldBeTrue();
    }

    [Fact]
    public async void LeaveMemberShouldCauseUpdateEvent()
    {
        // arrange
        var request = new CreateMatchHubRequest()
        {
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            Bet = new Bet(currencyId: 1, bet: 100)
        };

        var interactor = SharedServiceProvider.GetInstance()
            .GetRequiredService<MatchHubsInteractor>();

        IMatchHub hub = await interactor.CreateAsync(request);
        var memberId = Guid.NewGuid();
        bool updateEventRaised = false;
        var tcs = new TaskCompletionSource<bool>();

        EventHandler callback = delegate (object? sender, EventArgs e)
        {
            tcs.SetResult(true);
            updateEventRaised = true;
        };

        // act
        await interactor.JoinMemberAsync(memberId, hub.Id, roomIndex: 0);
        hub.Updated += callback;
        await interactor.LeaveMemberAsync(memberId, hub.Id);

        // assert
        updateEventRaised.ShouldBeTrue();
    }
}
