using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.Testing;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.UseCases;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.LobbyTest.UseCaseTests;
public class DeleteTests
{
    [Fact]
    public async Task ShouldBeDisposed()
    {
        // arrange
        var options = new LobbyOptions()
        {
            Bet = new Bet(),
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            GameConfiguration = new GameConfiguration() { MaxPlayers = 2 }
        };

        var factory = SharedServiceProvider.GetInstance()
            .GetRequiredService<ILobbyUseCasesFactory>();

        var lobby = await factory.Get<ICreateLobbyUseCase>().ExecuteAsync(options);

        // act
        await factory.Get<IDeleteLobbyUseCase>().ExecuteAsync(lobby.Id);

        //assert
        lobby.IsDisposed.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldRemoveFromStorage()
    {
        // arrange
        var storageMock = new LobbiesMock().Mock;
        var provider = SharedServiceProvider.GetInstance(s =>
        {
            s.AddScoped(p => storageMock.Object);
        });

        var options = new LobbyOptions()
        {
            Bet = new Bet(),
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            GameConfiguration = new GameConfiguration() { MaxPlayers = 2 }
        };

        var factory = provider.GetRequiredService<ILobbyUseCasesFactory>();
        var lobby = await factory.Get<ICreateLobbyUseCase>().ExecuteAsync(options);

        // act
        await factory.Get<IDeleteLobbyUseCase>().ExecuteAsync(lobby.Id);

        //assert
        storageMock.Verify(x => x.RemoveAsync(lobby.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowIfLobbyNotFound()
    {
        // arrange
        var factory = SharedServiceProvider.GetInstance()
            .GetRequiredService<ILobbyUseCasesFactory>();

        // act
        var exception = await Should.ThrowAsync<LobbyException>(
            () => factory.Get<IDeleteLobbyUseCase>()
            .ExecuteAsync(Guid.NewGuid()));

        //assert
        exception.ErrorCode.ShouldBe(Errors.Main.LobbyNotFound);
    }
}
