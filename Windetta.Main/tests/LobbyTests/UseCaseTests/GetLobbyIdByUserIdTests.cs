using Microsoft.Extensions.DependencyInjection;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.UseCases;

namespace Windetta.MainTests.LobbyTest.UseCaseTests;

public class GetLobbyIdByUserIdTests
{
    [Fact]
    public async Task ShouldCall_ILobbyUsersAssociations_Method()
    {
        // arrange
        var userId = Guid.NewGuid();
        var mock = new Mock<ILobbyUsersAssociations>();
        var provider = SharedServiceProvider.GetInstance(s =>
        {
            s.AddSingleton(p => mock.Object);
        });

        var factory = provider.GetRequiredService
            <ILobbyUseCasesFactory>();

        // act
        _ = await factory.Get<IGetLobbyIdByUserIdUseCase>()
            .ExecuteAsync(userId);

        //assert
        mock.Verify(x => x.GetLobbyId(userId), Times.Once);
    }
}
