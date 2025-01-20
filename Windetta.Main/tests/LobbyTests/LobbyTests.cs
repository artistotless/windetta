using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Testing;
using Windetta.Contracts;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.LobbyTest;
public class LobbyTests
{
    [Fact]
    public async Task ShouldCreatesRoomsAccordingGameConfig()
    {
        // arrange
        var gameCfg = new GameConfiguration()
        {
            MinPlayersInTeam = 1,
            MaxPlayersInTeam = (uint)Random.Shared.Next(10, 1000),
            MinTeams = 1,
            MaxTeams = (uint)Random.Shared.Next(1, 1000)
        };

        var sc = new[] { new SupportedCurrency(1, 1, 100000) };

        var provider = SharedServiceProvider.GetInstance((services) =>
        {
            services.AddSingleton((p) => new GamesRepositoryMock(gameCfg, sc).Mock.Object);
        });

        var request = new CreateLobbyDto()
        {
            GameId = ExampleGuids.GameId,
            Bet = new FundsInfo(currencyId: 1, amount: 100)
        };

        // act
        var interactor = provider.GetRequiredService<LobbiesInteractor>();

        ILobby lobby = await interactor.CreateAsync(request, ExampleGuids.UserId);

        // assert
        lobby.Rooms.Count().ShouldBe((int)gameCfg.MaxTeams);
        lobby.Rooms.First().MaxMembers.ShouldBe(gameCfg.MaxPlayersInTeam);
    }

    [Fact]
    public async void ShouldAddMemberToSelectedRoom()
    {
        // arrange
        var request = new CreateLobbyDto()
        {
            GameId = ExampleGuids.GameId,
            Bet = new FundsInfo(currencyId: 1, amount: 100)
        };

        var interactor = SharedServiceProvider.GetInstance()
            .GetRequiredService<LobbiesInteractor>();

        var memberId = Guid.NewGuid();

        // act
        ILobby lobby = await interactor.CreateAsync(request, ExampleGuids.UserId);
        var room = lobby.Rooms.First();
        await interactor.JoinMemberAsync(memberId, lobby.Id, room.Index);

        // assert
        room.Members.ShouldContain(x => x.Id == memberId);
    }

    [Fact]
    public async void JoinMemberShouldCauseUpdateEvent()
    {
        // arrange
        var request = new CreateLobbyDto()
        {
            GameId = ExampleGuids.GameId,
            Bet = new FundsInfo(currencyId: 1, amount: 100)
        };

        var interactor = SharedServiceProvider.GetInstance()
            .GetRequiredService<LobbiesInteractor>();

        ILobby lobby = await interactor.CreateAsync(request, ExampleGuids.UserId);
        var memberId = Guid.NewGuid();
        bool updateEventRaised = false;
        var tcs = new TaskCompletionSource<bool>();

        EventHandler callback = delegate (object? sender, EventArgs e)
        {
            tcs.SetResult(true);
            updateEventRaised = true;
        };

        lobby.Updated += callback;

        // act
        await interactor.JoinMemberAsync(memberId, lobby.Id, roomIndex: 0);

        // assert
        updateEventRaised.ShouldBeTrue();
    }

    [Fact]
    public async void LeaveMemberShouldCauseUpdateEvent()
    {
        // arrange
        var request = new CreateLobbyDto()
        {
            GameId = ExampleGuids.GameId,
            Bet = new FundsInfo(currencyId: 1, amount: 100)
        };

        var interactor = SharedServiceProvider.GetInstance()
            .GetRequiredService<LobbiesInteractor>();

        ILobby lobby = await interactor.CreateAsync(request, ExampleGuids.UserId);
        var memberId = Guid.NewGuid();
        bool updateEventRaised = false;

        EventHandler callback = delegate (object? sender, EventArgs e)
        {
            updateEventRaised = true;
        };

        // act
        await interactor.JoinMemberAsync(memberId, lobby.Id, roomIndex: 0);
        lobby.Updated += callback;
        await interactor.LeaveMemberAsync(memberId, lobby.Id);

        // assert
        updateEventRaised.ShouldBeTrue();
    }
}
