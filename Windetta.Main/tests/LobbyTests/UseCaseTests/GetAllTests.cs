using Microsoft.Extensions.DependencyInjection;
using Windetta.Main.Core.Lobbies.UseCases;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.LobbyTest.UseCaseTests;
public class GetAllTests
{
    [Fact]
    public async Task ShouldCallLobbiesMethod()
    {
        // arrange
        var storageMock = new LobbiesMock().Mock;
        var provider = SharedServiceProvider.GetInstance(s =>
        {
            s.AddSingleton(p => storageMock.Object);
        });

        var factory = provider.GetRequiredService
            <ILobbyUseCasesFactory>();

        // act
        _ = await factory.Get<IGetAllLobbiesUseCase>().ExecuteAsync();

        //assert
        storageMock.Verify(x => x.GetAllAsync(), Times.Once);
    }
}
