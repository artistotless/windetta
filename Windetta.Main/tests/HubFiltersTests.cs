using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.MatchHubs.Dtos;
using Windetta.Main.MatchHubs;
using Windetta.MainTests.Mocks;
using Windetta.MainTests.Shared;

namespace Windetta.MainTests;

public class HubFiltersTests
{
    [Fact]
    public async Task JoinShouldBeRejected_IfFilterDoesNotAllowToJoin()
    {
        // arrange
        var request = new CreateMatchHubRequest()
        {
            GameId = IdExamples.GameId,
            InitiatorId = IdExamples.UserId,
            Bet = new Bet(1, 100),
            JoinFilters = new[] { new PluginSetDto(nameof(AlwaysFalseJoinFilter)) }
        };

        var interactor = SharedServiceProvider.GetInstance()
            .GetRequiredService<MatchHubsInteractor>();

        var memberId = Guid.NewGuid();
        var hub = await interactor.CreateAsync(request);
        var room = hub.Rooms.First();

        // act
        var exception = await Should.ThrowAsync<MatchHubPluginException>(
            () => interactor.JoinMember(memberId, hub.Id, room.Id));

        // assert
        room.MembersCount.ShouldBe(1);
        exception.ErrorCode.ShouldMatch(Errors.Main.JoinFilterValidationFail);
        exception.Message!.ShouldMatch("join rejected");
    }
}
