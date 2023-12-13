using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Main.Core.Exceptions;

namespace Windetta.MainTests.MatchHubTest.UseCaseTests;

public class LeaveMemberTests
{
    [Fact]
    public async Task ShouldThrowIfHubNotFound()
    {
        // arrange
        var factory = SharedServiceProvider.GetInstance()
            .GetRequiredService<IMatchHubUseCasesFactory>();

        // act
        var exception = await Should.ThrowAsync<MatchHubException>(
            () => factory.Get<ILeaveMemberMatchHubUseCase>()
            .ExecuteAsync(userId: Guid.NewGuid(), hubId: Guid.NewGuid()));

        //assert
        exception.ErrorCode.ShouldBe(Errors.Main.MatchHubNotFound);
    }
}
