using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Main.Core.Exceptions;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.MatchHub.UseCaseTests;
public class GetTests
{
    [Fact]
    public async Task ShouldThrowIfHubNotFound()
    {
        // arrange
        var hubId = Guid.NewGuid();
        var storageMock = new MatchHubsMock().Mock;
        var provider = SharedServiceProvider.GetInstance(s =>
        {
            s.AddSingleton(p => storageMock.Object);
        });

        var factory = provider.GetRequiredService
            <IMatchHubUseCasesFactory>();

        // act
        var exception = await Should.ThrowAsync<MatchHubException>(
            () => factory.Get<IGetMatchHubUseCase>()
            .ExecuteAsync(hubId));

        //assert
        storageMock.Verify(x => x.GetAsync(hubId), Times.Once);
        exception.ErrorCode.ShouldBe(Errors.Main.MatchHubNotFound);
    }
}
