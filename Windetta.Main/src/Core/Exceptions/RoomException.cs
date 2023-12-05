using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Main.Core.Exceptions;

public class RoomException : WindettaException
{
    public static RoomException MemberNotInRoom
        => new RoomException(Errors.Main.MemberNotInRoom);

    public static RoomException MaxMembersInRoomReached
        => new RoomException(Errors.Main.MaxMembersInRoomReached);

    public static RoomException MemberAlreadyJoinedRoom
        => new RoomException(Errors.Main.MemberAlreadyJoinedRoom);

    private RoomException(string errorCode) : base(errorCode) { }
}
