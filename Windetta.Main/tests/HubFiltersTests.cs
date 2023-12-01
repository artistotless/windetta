using Windetta.Common.Constants;
using Windetta.Common.Types;
using Windetta.Main.MatchHub;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests;

public class HubFiltersTests
{
    [Fact]
    public async Task JoinShouldBeRejected_IfFilterDoesNotAllowToJoin()
    {
        // arrange
        var filter = new AlwaysFalseJoinFilter();
        var options = new MatchHubOptions()
        {
            Bet = new Bet(1, 100),
            GameConfiguration = new() { MaxPlayers = 2, },
            JoinFilters = new[] { filter.Name }
        };
        var interactor = new MatchHubsInteractor(new Mock<IMatchHubs>().Object, null, new[] { filter });
        var initiatorId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var hub = await interactor.CreateAsync(options, initiatorId);
        var room = hub.Rooms.First();

        // act
        var exception = await Should.ThrowAsync<WindettaException>(
            () => interactor.JoinMember(memberId, hub, room.Id));

        // assert
        room.MembersCount.ShouldBe(1);
        exception.ErrorCode.ShouldMatch(Errors.Main.JoinFilterValidationFail);
        exception.Message.ShouldMatch("join rejected");
    }
}
