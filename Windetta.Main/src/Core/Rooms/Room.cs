using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Main.Rooms;

public class Room : IDisposable
{
    //Events
    public event EventHandler<RoomEventArg> MemberJoined;
    public event EventHandler<RoomEventArg> MemberLeft;

    // Public fields
    public Guid Id { get; private set; }
    public IReadOnlyCollection<RoomMember> Members => _members;
    public uint MaxMembers { get; init; }
    public int MembersCount => Members.Count;


    // Private fields
    private List<RoomMember> _members;

    /// <summary>
    /// Room constructor
    /// </summary>
    /// <param name="maxMembers">0 - unlimit</param>
    public Room(uint maxMembers = 0) : this(Guid.NewGuid(), maxMembers) { }

    public Room(Guid id, uint maxMembers = 0)
    {
        Id = id;
        _members = new List<RoomMember>((int)maxMembers);
        MaxMembers = maxMembers;
    }

    internal void Add(RoomMember member)
    {
        if (MaxMembers != 0 && MembersCount == MaxMembers)
            throw new WindettaException(
                Errors.Main.MaxMembersInRoomReached);

        if (Members.Contains(member))
            throw new WindettaException(Errors.Main.MemberAlreadyJoined);

        _members.Add(member);

        OnMemberJoined(member);
    }

    internal void Remove(RoomMember member)
    {
        if (!Members.Contains(member))
            throw new WindettaException(Errors.Main.MemberNotInRoom);

        _members.Remove(member);

        OnMemberLeft(member);
    }

    public void Dispose()
    {
        _members = null;
    }

    private void OnMemberJoined(RoomMember member)
    {
        MemberJoined?.Invoke(this, new()
        {
            MemberId = member.Id
        });
    }

    private void OnMemberLeft(RoomMember member)
    {
        MemberLeft?.Invoke(this, new()
        {
            MemberId = member.Id
        });
    }
}