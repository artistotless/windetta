using Windetta.Common.Constants;
using Windetta.Common.Types;
using Windetta.Main.Core.Rooms;

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
        var member = new RoomMember(id: Guid.NewGuid());
        var room = new Room();

        // act
        member.Join(room);

        // assert
        room.MembersCount.ShouldBe(1);
    }

    [Fact]
    public void JoiningRoomShouldThrowsExceptionIfPlayerAlreadyJoinedRoom()
    {
        // arrange
        var member = new RoomMember(id: Guid.NewGuid());
        var room = new Room();

        // act
        member.Join(room);
        var exception = Should.Throw<WindettaException>(() => member.Join(room));

        // assert
        exception.ShouldNotBeNull();
        exception.ErrorCode.ShouldBe(Errors.Main.MemberAlreadyJoinedRoom);
    }

    [Fact]
    public void LeaveRoomShouldDecreasePlayersNumber()
    {
        // arrange
        var member = new RoomMember(id: Guid.NewGuid());
        var room = new Room();

        // act, assert
        member.Join(room);
        room.MembersCount.ShouldBe(1);
        member.LeaveRoom();
        room.MembersCount.ShouldBe(0);
    }

    [Fact]
    public void LeaveRoomShouldBeIdempotenticIfPlayerNotInRoom()
    {
        // arrange
        var member = new RoomMember(id: Guid.NewGuid());
        var room = new Room();

        // act
        var exception = Should.Throw<WindettaException>(() => member.LeaveRoom());

        // assert
        exception.ShouldNotBeNull();
        exception.ErrorCode.ShouldBe(Errors.Main.MemberNotInRoom);
    }

    [Fact]
    public void JoinRoomShouldThrowExceptionIfArgumentIsNull()
    {
        // arrange
        var member = new RoomMember(id: Guid.NewGuid());

        // act
        var exception = Should.Throw<ArgumentNullException>(
            () => member.Join(null));

        // assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public void JoinRoomShouldThrowExceptionIfMaxPlayersLimitReached()
    {
        // arrange
        var member = new RoomMember(id: Guid.NewGuid());
        var member2 = new RoomMember(id: Guid.NewGuid());
        var room = new Room(maxMembers: 1);

        // act
        member.Join(room);
        var exception = Should.Throw<WindettaException>(
            () => member2.Join(room));

        // assert
        exception.ShouldNotBeNull();
        exception.ErrorCode.ShouldBe(Errors.Main.MaxMembersInRoomReached);
    }

    [Fact]
    public void JoinRoomShouldRaiseEvent()
    {
        // arrange
        var member = new RoomMember(id: Guid.NewGuid());
        var room = new Room(maxMembers: 1);
        bool eventRaised = false;

        EventHandler<RoomEventArg> callback = (object? sender, RoomEventArg arg) =>
        {
            eventRaised = true;
        };

        room.MemberJoined += callback;

        // act
        member.Join(room);

        // assert
        eventRaised.ShouldBeTrue();
    }

    [Fact]
    public void LeaveRoomShouldRaiseEvent()
    {
        // arrange
        var member = new RoomMember(id: Guid.NewGuid());
        var room = new Room(maxMembers: 1);
        bool eventRaised = false;

        EventHandler<RoomEventArg> callback = (object? sender, RoomEventArg arg) =>
        {
            eventRaised = true;
        };

        room.MemberLeft += callback;

        // act
        member.Join(room);
        member.LeaveRoom();

        // assert
        eventRaised.ShouldBeTrue();
    }
}