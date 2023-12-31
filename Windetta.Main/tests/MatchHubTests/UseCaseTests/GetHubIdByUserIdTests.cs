﻿using Microsoft.Extensions.DependencyInjection;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Core.MatchHubs.UseCases;

namespace Windetta.MainTests.MatchHubTest.UseCaseTests;

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
