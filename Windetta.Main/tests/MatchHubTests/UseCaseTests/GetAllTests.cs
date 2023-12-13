using Microsoft.Extensions.DependencyInjection;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.MatchHubTest.UseCaseTests;
public class GetAllTests
{
    [Fact]
    public async Task ShouldCallMatchHubsMethod()
    {
        // arrange
        var storageMock = new MatchHubsMock().Mock;
        var provider = SharedServiceProvider.GetInstance(s =>
        {
            s.AddSingleton(p => storageMock.Object);
        });

        var factory = provider.GetRequiredService
            <IMatchHubUseCasesFactory>();

        // act
        _ = await factory.Get<IGetAllMatchHubsUseCase>().ExecuteAsync();

        //assert
        storageMock.Verify(x => x.GetAllAsync(), Times.Once);
    }
}
