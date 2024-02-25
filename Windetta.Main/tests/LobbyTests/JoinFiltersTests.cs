using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.Testing;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.LobbyTest;

public class JoinFiltersTests
{
    [Fact]
    public async Task JoinShouldBeRejected_IfFilterDoesNotAllowToJoin()
    {
        // arrange
        var request = new CreateLobbyDto()
        {
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId,
            Bet = new Bet(1, 100),
            JoinFilters = new[] { new PluginSetDto(nameof(AlwaysFalseJoinFilter)) }
        };

        var interactor = SharedServiceProvider.GetInstance()
            .GetRequiredService<LobbiesInteractor>();

        var memberId = Guid.NewGuid();
        var lobby = await interactor.CreateAsync(request);
        var room = lobby.Rooms.First();

        // act
        var exception = await Should.ThrowAsync<LobbyPluginException>(
            () => interactor.JoinMemberAsync(memberId, lobby.Id, roomIndex: 0));

        // assert
        room.MembersCount.ShouldBe(1);
        exception.ErrorCode.ShouldMatch(Errors.Main.JoinFilterValidationFail);
        exception.Message!.ShouldMatch("join rejected");
    }

    //[Fact]
    //public async Joi
}
