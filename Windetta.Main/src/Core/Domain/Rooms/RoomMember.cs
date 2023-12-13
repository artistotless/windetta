using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Main.Core.Domain.Rooms;

public sealed class RoomMember
{
    public Guid Id { get; init; }

    private Room? _room;

    public RoomMember(Guid id)
    {
        Id = id;
    }

    public void Join(Room room)
    {
        if (room is null)
            throw new ArgumentNullException(nameof(room));

        room.Add(this);

        _room = room;
    }

    public void LeaveRoom()
    {
        if (_room is null)
            throw new WindettaException(Errors.Main.MemberNotInRoom);

        _room.Remove(this);
    }
}