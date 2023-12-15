using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Core.MatchHubs.Plugins;
using Windetta.Main.Core.MatchHubs.UseCases;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.MatchHubTest.UseCaseTests;

public class JoinMemberTests
{
    [Fact]
    public async Task ShouldThrowIfHubNotFound()
    {
        // arrange
        var factory = SharedServiceProvider.GetInstance()
            .GetRequiredService<IMatchHubUseCasesFactory>();

        // act
        var exception = await Should.ThrowAsync<MatchHubException>(
            () => factory.Get<IJoinMemberMatchHubUseCase>()
            .ExecuteAsync(userId: Guid.NewGuid(), hubId: Guid.NewGuid(), roomId: Guid.NewGuid()));

        //assert
        exception.ErrorCode.ShouldBe(Errors.Main.MatchHubNotFound);
    }

    [Fact]
    public async Task ShouldThrowIfFundsNotEnough()
    {
        // arrange
        var options = new MatchHubOptions()
        {
            InitiatorId = IdExamples.UserId,
            GameConfiguration = new GameConfiguration() { MaxPlayers = 2 },
        };
        var hub = new ProxyMatchHub(options);
        var storageMock = new MatchHubsMock() { ReturnThisHub = hub }.Mock;
        var walletMock = new WalletServiceMock() { ReturnIsEqualOrGreater = false }.Mock;
        var provider = SharedServiceProvider.GetInstance(s =>
        {
            s.AddSingleton(p => storageMock.Object);
            s.AddScoped(p => walletMock.Object);
        });

        var factory = provider.GetRequiredService
            <IMatchHubUseCasesFactory>();

        // act
        var exception = await Should.ThrowAsync<WalletException>(
            () => factory.Get<IJoinMemberMatchHubUseCase>()
            .ExecuteAsync(userId: Guid.NewGuid(), hubId: hub.Id, roomId: hub.Rooms.First().Id));

        //assert
        exception.ErrorCode.ShouldBe(Errors.Wallet.FundsNotEnough);
    }

    [Fact]
    public async Task ShouldExecuteFiltersIfDefined()
    {
        // arrange
        var options = new MatchHubOptions()
        {
            Bet = new Bet(),
            GameId = IdExamples.GameId,
            InitiatorId = IdExamples.UserId,
            GameConfiguration = new GameConfiguration() { MaxPlayers = 2 },
        };

        var hubMock = new Mock<ProxyMatchHub>(options, null);
        hubMock.Setup(x => x.GetJoinFilters())
            .Returns(() => new List<JoinFilter>() { new AlwaysFalseJoinFilter() });


        var storageMock = new MatchHubsMock() { ReturnThisHub = hubMock.Object }.Mock;
        var walletMock = new WalletServiceMock() { ReturnIsEqualOrGreater = true }.Mock;
        var provider = SharedServiceProvider.GetInstance(s =>
        {
            s.AddSingleton(p => storageMock.Object);
            s.AddScoped(p => walletMock.Object);
        });

        var factory = provider.GetRequiredService
            <IMatchHubUseCasesFactory>();

        // act
        var exception = await Should.ThrowAsync<MatchHubPluginException>(
            () => factory.Get<IJoinMemberMatchHubUseCase>()
            .ExecuteAsync(
                userId: Guid.NewGuid(),
                hubId: hubMock.Object.Id,
                roomId: hubMock.Object.Rooms.First().Id));

        //assert
        exception.ErrorCode.ShouldBe(Errors.Main.JoinFilterValidationFail);
        hubMock.Verify(x => x.GetJoinFilters(), Times.Once);
    }
}
