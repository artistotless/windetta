﻿using Windetta.Main.Rooms;

namespace Windetta.Main.MatchHub;

public interface IMatchHub : IHubDisposeListener, IHubReadyListener, IDisposable
{
    Guid Id { get; }

    event EventHandler Updated;
    event EventHandler Disposed;
    event EventHandler Ready;

    IReadOnlyCollection<string>? JoinFilters { get; init; }

    internal void Add(RoomMember member, Guid roomId);
    internal void Remove(Guid memberId);
}