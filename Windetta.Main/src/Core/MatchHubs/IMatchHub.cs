using Windetta.Main.MatchHubs.Filters;
using Windetta.Main.Rooms;

namespace Windetta.Main.MatchHubs;

public interface IMatchHub : IHubDisposeListener, IHubReadyListener, IDisposable
{
    Guid Id { get; }
    Guid GameId { get; init; }

    Guid InitiatorId { get; init; }

    MatchHubState State { get; protected set; }

    // Events
    event EventHandler Updated;
    event EventHandler Disposed;
    event EventHandler Ready;

    // Plugins
    IEnumerable<string>? JoinFilters { get; }
    public string? AutoReadyStrategy { get; }
    public string? AutoDisposeStrategy { get; }

    internal void Add(RoomMember member, Guid roomId);
    internal void Remove(Guid memberId);
    internal IEnumerable<IJoinFilter>? GetJoinFilters();
}
