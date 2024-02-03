using Windetta.Main.Core.Lobbies.Plugins;
using Windetta.Main.Core.Rooms;

namespace Windetta.Main.Core.Lobbies;

public interface ILobby : ILobbyDisposeListener, ILobbyReadyListener, IDisposable
{
    Guid Id { get; }
    Guid GameId { get; init; }

    Guid InitiatorId { get; init; }
    IReadOnlyDictionary<string, string> Properties { get; init; }
    LobbyState State { get; protected set; }
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
    void Add(RoomMember member, ushort roomIndex);
    void Remove(Guid memberId, ushort roomIndex);
}
