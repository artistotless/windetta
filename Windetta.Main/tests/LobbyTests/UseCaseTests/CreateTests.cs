using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.Testing;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Plugins;
using Windetta.Main.Core.Lobbies.UseCases;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.LobbyTest.UseCaseTests;
public class CreateTests
{
    [Fact]
    public async Task ShouldThrowExceptionIfFundsNotEnough()
    {
        // arrange
        var walletService = new WalletServiceMock() { ReturnIsEqualOrGreater = false };
        var provider = SharedServiceProvider.GetInstance((s) =>
        {
            s.AddScoped(p => walletService.Mock.Object);
        });

        var options = new LobbyOptions()
        {
            Bet = new Bet(currencyId: 1, bet: 100_0),
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            GameConfiguration = new() { MaxPlayersInTeam = 2 },
        };

        var useCase = provider
            .GetRequiredService<ILobbyUseCasesFactory>()
            .Get<ICreateLobbyUseCase>();

        // act
        var exception = await Should.ThrowAsync<WalletException>(
            () => useCase.ExecuteAsync(options));

        // assert
        exception.ErrorCode.ShouldBe(Errors.Wallet.FundsNotEnough);
    }

    [Fact]
    public void Should_CreateLobby_IfFundsEnough()
    {
        // arrange
        var walletService = new WalletServiceMock() { ReturnIsEqualOrGreater = true };
        var provider = SharedServiceProvider.GetInstance((s) =>
        {
            s.AddScoped(p => walletService.Mock.Object);
        });

        var options = new LobbyOptions()
        {
            Bet = new Bet(currencyId: 1, bet: 100_0),
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            GameConfiguration = new() { MaxPlayersInTeam = 2 },
        };

        var useCase = provider
            .GetRequiredService<ILobbyUseCasesFactory>()
            .Get<ICreateLobbyUseCase>();

        // act
        var lobby = Should.NotThrow(() => useCase.ExecuteAsync(options));

        // assert
        lobby.ShouldNotBeNull();
        lobby.Bet.ShouldBe(options.Bet);
    }

    [Fact]
    public void Should_UseDefaultStrategiesIfNotProvided()
    {
        // arrange
        var walletService = new WalletServiceMock() { ReturnIsEqualOrGreater = true };
        var provider = SharedServiceProvider.GetInstance((s) =>
        {
            s.AddScoped(p => walletService.Mock.Object);
        });

        var options = new LobbyOptions()
        {
            Bet = new Bet(currencyId: 1, bet: 100_0),
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            GameConfiguration = new() { MaxPlayersInTeam = 2 },
        };

        var useCase = provider
            .GetRequiredService<ILobbyUseCasesFactory>()
            .Get<ICreateLobbyUseCase>();

        // act
        var lobby = Should.NotThrow(() => useCase.ExecuteAsync(options));

        // assert
        lobby.AutoReadyStrategy.ShouldBe(nameof(DefaultReadyStrategy));
        lobby.AutoDisposeStrategy.ShouldBe(nameof(DefaultDisposeStrategy));
    }
}
