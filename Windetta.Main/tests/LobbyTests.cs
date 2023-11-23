using AutoFixture;
using Moq;
using Windetta.Main.Games;
using Windetta.Main.Rooms;

namespace Windetta.MainTests;

public class LobbyTests
{
    [Fact]
    public async Task Test()
    {
        var config = new GameConfiguration()
        {
            MinPlayers = 1,
            MaxPlayers = 100,
            MinTeams = 1,
            MaxTeams = 100,
        };

        var gamesMock = new Mock<IGames>();

        gamesMock
        .Setup(x => x.Get(It.IsAny<Guid>()))
        .ReturnsAsync((Guid id) => new Fixture()
        .Build<Game>()
        .With(x => x.Id, id)
        .With(x => x.Configuration, config)
        .Create());

        var game = await gamesMock.Object.Get(Guid.NewGuid());
        var room = new Room(game.Configuration.MaxPlayers);
        var member1 = new RoomMember(Guid.NewGuid());
        var member2 = new RoomMember(Guid.NewGuid());

        member1.Join(room);
        member2.Join(room);

        ;
    }
}
