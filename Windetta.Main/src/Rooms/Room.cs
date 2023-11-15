using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Main.Rooms;

public class Room : IDisposable
{
    public IReadOnlyCollection<RoomMember> Members => _members;

    public uint MaxMembers;
    public int MembersCount => Members.Count;

    private List<RoomMember> _members;

    /// <summary>
    /// Room constructor
    /// </summary>
    /// <param name="maxMembers">0 - unlimit</param>
    public Room(uint maxMembers = 0)
    {
        _members = new List<RoomMember>((int)maxMembers);
        MaxMembers = maxMembers;
    }

    internal void Add(RoomMember player)
    {
        if (MaxMembers != 0 && MembersCount == MaxMembers)
            throw new WindettaException(
                Errors.Main.MaxMembersInRoomReached);

        if (Members.Contains(player))
            throw new WindettaException(Errors.Main.MemberAlreadyJoined);

        _members.Add(player);
    }

    internal void Remove(RoomMember player)
    {
        if (!Members.Contains(player))
            throw new WindettaException(Errors.Main.MemberNotInRoom);

        _members.Remove(player);
    }

    public void Dispose()
    {
        _members = null;
    }
}