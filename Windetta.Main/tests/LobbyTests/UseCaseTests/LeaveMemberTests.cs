using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Lobbies.UseCases;

namespace Windetta.MainTests.LobbyTest.UseCaseTests;

public class LeaveMemberTests
{
    [Fact]
    public async Task ShouldThrowIfLobbyNotFound()
    {
        // arrange
        var factory = SharedServiceProvider.GetInstance()
            .GetRequiredService<ILobbyUseCasesFactory>();

        // act
        var exception = await Should.ThrowAsync<LobbyException>(
            () => factory.Get<ILeaveMemberLobbyUseCase>()
            .ExecuteAsync(userId: Guid.NewGuid(), lobbyId: Guid.NewGuid()));

        //assert
        exception.ErrorCode.ShouldBe(Errors.Main.LobbyNotFound);
    }
}
