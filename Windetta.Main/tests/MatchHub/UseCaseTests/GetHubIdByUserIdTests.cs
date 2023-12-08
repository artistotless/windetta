using Microsoft.Extensions.DependencyInjection;

namespace Windetta.MainTests.MatchHub.UseCaseTests;

public class GetHubIdByUserIdTests
{
    [Fact]
    public async Task ShouldCall_IMatchHubUsersAssociations_Method()
    {
        // arrange
        var userId = Guid.NewGuid();
        var mock = new Mock<IMatchHubUsersAssociations>();
        var provider = SharedServiceProvider.GetInstance(s =>
        {
            s.AddSingleton(p => mock.Object);
        });

        var factory = provider.GetRequiredService
            <IMatchHubUseCasesFactory>();

        // act
        _ = await factory.Get<IGetMatchHubIdByUserIdUseCase>()
            .ExecuteAsync(userId);

        //assert
        mock.Verify(x => x.GetHubId(userId), Times.Once);
    }
}
