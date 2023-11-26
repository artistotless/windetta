using Windetta.Main.Games;
using Windetta.Main.MatchHub;
using Windetta.Main.Services;

namespace Windetta.MainTests;
public class HubTests
{
    [Fact]
    public async Task ShouldCreatesRoomsAccordingGameConfig()
    {
        // arrange
        var config = new GameConfiguration()
        {
            MinPlayers = 1,
            MaxPlayers = 100,
            MinTeams = 1,
            MaxTeams = 100,
        };

        var options = new MatchHubOptions()
        {
            GameConfiguration = config,
            Bet = new Bet(currencyId: 1, bet: 100)
        };

        // act
        var interactor = new MatchHubsInteractor(new Mock<IMatchHubs>().Object);

        IMatchHub hub = await interactor.Create(options);

        // assert
        hub.Rooms.Count().ShouldBe((int)config.MaxTeams);
    }

    [Fact]
    public async void ShouldAddMemberToSelectedRoom()
    {
        // arrange
        var config = new GameConfiguration()
        {
            MinPlayers = 1,
            MaxPlayers = 100,
        };

        var options = new MatchHubOptions()
        {
            GameConfiguration = config,
            Bet = new Bet(currencyId: 1, bet: 100)
        };

        var interactor = new MatchHubsInteractor(new Mock<IMatchHubs>().Object);

        IMatchHub hub = await interactor.Create(options);
        var userId = Guid.NewGuid();
        var room = hub.Rooms.First();

        // act
        interactor.JoinMember(userId, hub, room.Id);

        // assert
        room.Members.ShouldContain(x => x.Id == userId);
    }

    [Fact]
    public async void JoinMemberShouldCauseUpdateEvent()
    {
        // arrange
        var config = new GameConfiguration()
        {
            MinPlayers = 1,
            MaxPlayers = 100,
        };

        var options = new MatchHubOptions()
        {
            GameConfiguration = config,
            Bet = new Bet(currencyId: 1, bet: 100)
        };

        var interactor = new MatchHubsInteractor(new Mock<IMatchHubs>().Object);

        IMatchHub hub = await interactor.Create(options);
        var memberId = Guid.NewGuid();
        var roomId = hub.Rooms.First().Id;
        bool updateEventRaised = false;
        var tcs = new TaskCompletionSource<bool>();

        EventHandler callback = delegate (object? sender, EventArgs e)
        {
            tcs.SetResult(true);
            updateEventRaised = true;
        };

        hub.Updated += callback;

        // act
        interactor.JoinMember(memberId, hub, roomId);

        // assert
        updateEventRaised.ShouldBeTrue();
    }

    [Fact]
    public async void LeaveMemberShouldCauseUpdateEvent()
    {
        // arrange
        var config = new GameConfiguration()
        {
            MinPlayers = 1,
            MaxPlayers = 100,
        };

        var options = new MatchHubOptions()
        {
            GameConfiguration = config,
            Bet = new Bet(currencyId: 1, bet: 100)
        };

        var interactor = new MatchHubsInteractor(new Mock<IMatchHubs>().Object);

        IMatchHub hub = await interactor.Create(options);
        var memberId = Guid.NewGuid();
        var roomId = hub.Rooms.First().Id;
        bool updateEventRaised = false;
        var tcs = new TaskCompletionSource<bool>();

        EventHandler callback = delegate (object? sender, EventArgs e)
        {
            tcs.SetResult(true);
            updateEventRaised = true;
        };

        // act
        interactor.JoinMember(memberId, hub, roomId);
        hub.Updated += callback;
        interactor.LeaveMember(memberId, hub);

        // assert
        updateEventRaised.ShouldBeTrue();
    }


    #region Helpers
    private IWalletService GetWalletService()
    {
        var walletMock = new Mock<IWalletService>();

        walletMock
            .Setup(x => x.HoldBalance(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<ulong>()))
            .Returns(Task.CompletedTask);
        walletMock
            .Setup(x => x.UnHoldBalance(It.IsAny<Guid>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);
        walletMock
            .Setup(x => x.GetBalance(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(new UserBalance()
            {
                Amount = 100,
                HeldAmount = 0
            });

        return walletMock.Object;
    }

    private IGames GetGamesService(GameConfiguration config)
    {
        var gamesMock = new Mock<IGames>();

        gamesMock
        .Setup(x => x.Get(It.IsAny<Guid>()))
        .ReturnsAsync((Guid id) => new Fixture()
        .Build<Game>()
        .With(x => x.Id, id)
        .With(x => x.Configuration, config)
        .Create());

        return gamesMock.Object;
    }
    #endregion
}
