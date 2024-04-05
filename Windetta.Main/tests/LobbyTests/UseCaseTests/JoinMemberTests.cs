using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.Testing;
using Windetta.Contracts;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Plugins;
using Windetta.Main.Core.Lobbies.UseCases;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.LobbyTest.UseCaseTests;

public class JoinMemberTests
{
    [Fact]
    public async Task ShouldThrowIfLobbyNotFound()
    {
        // arrange
        var factory = SharedServiceProvider.GetInstance()
            .GetRequiredService<ILobbyUseCasesFactory>();

        // act
        var exception = await Should.ThrowAsync<LobbyException>(
            () => factory.Get<IJoinMemberLobbyUseCase>()
            .ExecuteAsync(userId: Guid.NewGuid(), lobbyId: Guid.NewGuid(), roomIndex: 0));

        //assert
        exception.ErrorCode.ShouldBe(Errors.Main.LobbyNotFound);
    }

    [Fact]
    public async Task ShouldThrowIfFundsNotEnough()
    {
        // arrange
        var options = new LobbyOptions()
        {
            InitiatorId = ExampleGuids.UserId,
            GameConfiguration = new GameConfiguration() { MaxPlayersInTeam = 2 },
        };
        var lobby = new ProxyLobby(options);
        var storageMock = new LobbiesMock() { ReturnThisLobby = lobby }.Mock;
        var walletMock = new WalletServiceMock() { ReturnIsEqualOrGreater = false }.Mock;
        var provider = SharedServiceProvider.GetInstance(s =>
        {
            s.AddSingleton(p => storageMock.Object);
            s.AddScoped(p => walletMock.Object);
        });

        var factory = provider.GetRequiredService
            <ILobbyUseCasesFactory>();

        // act
        var exception = await Should.ThrowAsync<WalletException>(
            () => factory.Get<IJoinMemberLobbyUseCase>()
            .ExecuteAsync(userId: Guid.NewGuid(), lobbyId: lobby.Id, roomIndex: 0));

        //assert
        exception.ErrorCode.ShouldBe(Errors.Wallet.FundsNotEnough);
    }

    [Fact]
    public async Task ShouldExecuteFiltersIfDefined()
    {
        // arrange
        var options = new LobbyOptions()
        {
            Bet = new FundsInfo(1, 100),
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            GameConfiguration = new GameConfiguration() { MaxPlayersInTeam = 2 },
        };

        var lobbyMock = new Mock<ProxyLobby>(options, null);
        lobbyMock.Setup(x => x.GetJoinFilters())
            .Returns(() => new List<JoinFilter>() { new AlwaysFalseJoinFilter() });


        var storageMock = new LobbiesMock() { ReturnThisLobby = lobbyMock.Object }.Mock;
        var walletMock = new WalletServiceMock() { ReturnIsEqualOrGreater = true }.Mock;
        var provider = SharedServiceProvider.GetInstance(s =>
        {
            s.AddSingleton(p => storageMock.Object);
            s.AddScoped(p => walletMock.Object);
        });

        var factory = provider.GetRequiredService
            <ILobbyUseCasesFactory>();

        // act
        var exception = await Should.ThrowAsync<LobbyPluginException>(
            () => factory.Get<IJoinMemberLobbyUseCase>()
            .ExecuteAsync(
                userId: Guid.NewGuid(),
                lobbyId: lobbyMock.Object.Id,
                roomIndex: 0));

        //assert
        exception.ErrorCode.ShouldBe(Errors.Main.JoinFilterValidationFail);
        lobbyMock.Verify(x => x.GetJoinFilters(), Times.Once);
    }
}
