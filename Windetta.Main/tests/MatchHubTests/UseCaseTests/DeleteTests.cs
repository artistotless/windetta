using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.Testing;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Core.MatchHubs.UseCases;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.MatchHubTest.UseCaseTests;
public class DeleteTests
{
    [Fact]
    public async Task ShouldBeDisposed()
    {
        // arrange
        var options = new MatchHubOptions()
        {
            Bet = new Bet(),
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            GameConfiguration = new GameConfiguration() { MaxPlayers = 2 }
        };

        var factory = SharedServiceProvider.GetInstance()
            .GetRequiredService<IMatchHubUseCasesFactory>();

        var hub = await factory.Get<ICreateMatchHubUseCase>().ExecuteAsync(options);

        // act
        await factory.Get<IDeleteMatchHubUseCase>().ExecuteAsync(hub.Id);

        //assert
        hub.IsDisposed.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldRemoveFromStorage()
    {
        // arrange
        var storageMock = new MatchHubsMock().Mock;
        var provider = SharedServiceProvider.GetInstance(s =>
        {
            s.AddScoped(p => storageMock.Object);
        });

        var options = new MatchHubOptions()
        {
            Bet = new Bet(),
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            GameConfiguration = new GameConfiguration() { MaxPlayers = 2 }
        };

        var factory = provider.GetRequiredService<IMatchHubUseCasesFactory>();
        var hub = await factory.Get<ICreateMatchHubUseCase>().ExecuteAsync(options);

        // act
        await factory.Get<IDeleteMatchHubUseCase>().ExecuteAsync(hub.Id);

        //assert
        storageMock.Verify(x => x.RemoveAsync(hub.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowIfHubNotFound()
    {
        // arrange
        var factory = SharedServiceProvider.GetInstance()
            .GetRequiredService<IMatchHubUseCasesFactory>();

        // act
        var exception = await Should.ThrowAsync<MatchHubException>(
            () => factory.Get<IDeleteMatchHubUseCase>()
            .ExecuteAsync(Guid.NewGuid()));

        //assert
        exception.ErrorCode.ShouldBe(Errors.Main.MatchHubNotFound);
    }
}
