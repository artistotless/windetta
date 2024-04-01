using Windetta.Common.Constants;
using Windetta.Common.Helpers;
using Windetta.Common.Types;

namespace Windetta.Main.Core.Rooms;

public sealed class RoomMember : IEquatable<RoomMember>
{
    public Guid Id { get; init; }
    public string Name { get; init; }

    private Room? _room;

    public RoomMember(Guid id, string? name = null)
    {
        Id = id;
        Name = name ?? $"Player#{id.Cut(6)}";
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

    public bool Equals(RoomMember? other)
        => Id == other?.Id;
}