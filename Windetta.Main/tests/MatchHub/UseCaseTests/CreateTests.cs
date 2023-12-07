using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Main.Core.Exceptions;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.MatchHub.UseCaseTests;

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

        var options = new MatchHubOptions()
        {
            Bet = new Bet(currencyId: 1, bet: 100_0),
            GameId = IdExamples.GameId,
            InitiatorId = IdExamples.UserId,
            GameConfiguration = new() { MaxPlayers = 2 },
        };

        var useCase = provider
            .GetRequiredService<IMatchHubUseCasesFactory>()
            .Get<ICreateMatchHubUseCase>();

        // act
        var exception = await Should.ThrowAsync<WalletException>(
            () => useCase.ExecuteAsync(options));

        // assert
        exception.ErrorCode.ShouldBe(Errors.Wallet.FundsNotEnough);
    }

    [Fact]
    public void Should_CreateHub_IfFundsEnough()
    {
        // arrange
        var walletService = new WalletServiceMock() { ReturnIsEqualOrGreater = true };
        var provider = SharedServiceProvider.GetInstance((s) =>
        {
            s.AddScoped(p => walletService.Mock.Object);
        });

        var options = new MatchHubOptions()
        {
            Bet = new Bet(currencyId: 1, bet: 100_0),
            GameId = IdExamples.GameId,
            InitiatorId = IdExamples.UserId,
            GameConfiguration = new() { MaxPlayers = 2 },
        };

        var useCase = provider
            .GetRequiredService<IMatchHubUseCasesFactory>()
            .Get<ICreateMatchHubUseCase>();

        // act
        var hub = Should.NotThrow(() => useCase.ExecuteAsync(options));

        // assert
        hub.ShouldNotBeNull();
        hub.Bet.ShouldBe(options.Bet);
    }

    [Fact]
    public void Should_CreateTournamentHub_IfPassCorrespondingOptions()
    {
        // arrange
        var walletService = new WalletServiceMock() { ReturnIsEqualOrGreater = true };
        var provider = SharedServiceProvider.GetInstance((s) =>
        {
            s.AddScoped(p => walletService.Mock.Object);
        });

        var options = new TournamentMatchHubOptions()
        {
            Bet = new Bet(currencyId: 1, bet: 100_0),
            GameId = IdExamples.GameId,
            InitiatorId = IdExamples.UserId,
            GameConfiguration = new() { MaxPlayers = 2 },
        };

        var useCase = provider
            .GetRequiredService<IMatchHubUseCasesFactory>()
            .Get<ICreateMatchHubUseCase>();

        // act
        var hub = Should.NotThrow(() => useCase.ExecuteAsync(options));

        // assert
        hub.ShouldBeOfType<TournamentMatchHub>();
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

        var options = new MatchHubOptions()
        {
            Bet = new Bet(currencyId: 1, bet: 100_0),
            GameId = IdExamples.GameId,
            InitiatorId = IdExamples.UserId,
            GameConfiguration = new() { MaxPlayers = 2 },
        };

        var useCase = provider
            .GetRequiredService<IMatchHubUseCasesFactory>()
            .Get<ICreateMatchHubUseCase>();

        // act
        var hub = Should.NotThrow(() => useCase.ExecuteAsync(options));

        // assert
        hub.AutoReadyStrategy.ShouldBe(nameof(DefaultReadyStrategy));
        hub.AutoDisposeStrategy.ShouldBe(nameof(DefaultDisposeStrategy));
    }
}
