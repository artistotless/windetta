using Windetta.Main.Core.MatchHubs.Plugins;
using Windetta.Main.Core.Rooms;

namespace Windetta.Main.Core.MatchHubs;

public interface IMatchHub : IHubDisposeListener, IHubReadyListener, IDisposable
{
    Guid Id { get; }
    Guid GameId { get; init; }

    Guid InitiatorId { get; init; }
    new int MembersCount { get; }
    MatchHubState State { get; protected set; }
    bool IsDisposed { get; }

    // Events
    event EventHandler Updated;
    event EventHandler Disposed;
    event EventHandler Ready;

    // Plugins
    IEnumerable<string>? JoinFilters { get; }
    public string? AutoReadyStrategy { get; }
    public string? AutoDisposeStrategy { get; }

    public IEnumerable<IJoinFilter>? GetJoinFilters();
    internal protected void Add(RoomMember member, Guid roomId);
    internal protected void Remove(Guid memberId);
}
