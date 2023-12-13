using Windetta.Main.Core.Domain.MatchHubs.Plugins;
using Windetta.Main.Core.Domain.Rooms;

namespace Windetta.Main.Core.Domain.MatchHubs;

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

    IEnumerable<IJoinFilter>? GetJoinFilters();
    void Add(RoomMember member, Guid roomId);
    void Remove(Guid memberId);
}
