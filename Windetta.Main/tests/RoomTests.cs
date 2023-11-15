using Windetta.Common.Constants;
using Windetta.Common.Types;
using Windetta.Main.Rooms;

namespace Windetta.MainTests;

public class RoomTests
{
    [Fact]
    public void OnceCreateRoom_PlayersNumberShouldbeZero()
    {
        var room = new Room();

        room.MembersCount.ShouldBe(0);
    }

    [Fact]
    public void JoiningRoomShouldIncreasePlayersNumber()
    {
        // arrange
        var player = new RoomMember(id: Guid.NewGuid());
        var room = new Room();

        // act
        player.Join(room);

        // assert
        room.MembersCount.ShouldBe(1);
    }

    [Fact]
    public void JoiningRoomShouldThrowsExceptionIfPlayerAlreadyJoinedRoom()
    {
        // arrange
        var player = new RoomMember(id: Guid.NewGuid());
        var room = new Room();

        // act
        player.Join(room);
        var exception = Should.Throw<WindettaException>(() => player.Join(room));

        // assert
        exception.ShouldNotBeNull();
        exception.ErrorCode.ShouldBe(Errors.Main.MemberAlreadyJoined);
    }

    [Fact]
    public void LeaveRoomShouldDecreasePlayersNumber()
    {
        // arrange
        var player = new RoomMember(id: Guid.NewGuid());
        var room = new Room();

        // act, assert
        player.Join(room);
        room.MembersCount.ShouldBe(1);
        player.LeaveRoom();
        room.MembersCount.ShouldBe(0);
    }

    [Fact]
    public void LeaveRoomShouldBeIdempotenticIfPlayerNotInRoom()
    {
        // arrange
        var player = new RoomMember(id: Guid.NewGuid());
        var room = new Room();

        // act
        var exception = Should.Throw<WindettaException>(() => player.LeaveRoom());

        // assert
        exception.ShouldNotBeNull();
        exception.ErrorCode.ShouldBe(Errors.Main.MemberNotInRoom);
    }

    [Fact]
    public void JoinRoomShouldThrowExceptionIfArgumentIsNull()
    {
        // arrange
        var player = new RoomMember(id: Guid.NewGuid());

        // act
        var exception = Should.Throw<ArgumentNullException>(
            () => player.Join(null));

        // assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public void JoinRoomShouldThrowExceptionIfMaxPlayersLimitReached()
    {
        // arrange
        var player = new RoomMember(id: Guid.NewGuid());
        var player2 = new RoomMember(id: Guid.NewGuid());
        var room = new Room(maxMembers: 1);

        // act
        player.Join(room);
        var exception = Should.Throw<WindettaException>(
            () => player2.Join(room));

        // assert
        exception.ShouldNotBeNull();
        exception.ErrorCode.ShouldBe(Errors.Main.MaxMembersInRoomReached);
    }
}