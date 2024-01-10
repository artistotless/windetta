using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Lobbies.UseCases;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.LobbyTest.UseCaseTests;
public class GetTests
{
    [Fact]
    public async Task ShouldThrowIfLobbyNotFound()
    {
        // arrange
        var lobbyId = Guid.NewGuid();
        var storageMock = new LobbiesMock().Mock;
        var provider = SharedServiceProvider.GetInstance(s =>
        {
            s.AddSingleton(p => storageMock.Object);
        });

        var factory = provider.GetRequiredService
            <ILobbyUseCasesFactory>();

        // act
        var exception = await Should.ThrowAsync<LobbyException>(
            () => factory.Get<IGetLobbyUseCase>()
            .ExecuteAsync(lobbyId));

        //assert
        storageMock.Verify(x => x.GetAsync(lobbyId), Times.Once);
        exception.ErrorCode.ShouldBe(Errors.Main.LobbyNotFound);
    }
}
