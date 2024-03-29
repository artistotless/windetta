﻿using Windetta.Main.Core.Exceptions;

namespace Windetta.Main.Core.Rooms;

public class Room : IDisposable
{
    //Events
    public event EventHandler<RoomEventArg> MemberJoined;
    public event EventHandler<RoomEventArg> MemberLeft;

    // Public fields
    public ushort Index { get; init; }
    public IReadOnlyCollection<RoomMember> Members => _members;
    public uint MaxMembers { get; init; }
    public int MembersCount => Members.Count;


    // Private fields
    private List<RoomMember> _members;

    /// <summary>
    /// Room constructor
    /// </summary>
    /// <param name="maxMembers">0 - unlimit</param>

    public Room(ushort index = 0, uint maxMembers = 0, IEnumerable<RoomMember>? members = null)
    {
        Index = index;

        if (members is not null)
            _members = members.ToList();
        else
            _members = new List<RoomMember>((int)maxMembers);

        MaxMembers = maxMembers;
    }

    internal void Add(RoomMember member)
    {
        if (MaxMembers != 0 && MembersCount == MaxMembers)
            throw RoomException.MaxMembersInRoomReached;

        if (Members.Contains(member))
            throw RoomException.MemberAlreadyJoinedRoom;

        _members.Add(member);

        OnMemberJoined(member);
    }

    internal void Remove(RoomMember member)
    {
        if (!Members.Contains(member))
            throw RoomException.MemberNotInRoom;

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