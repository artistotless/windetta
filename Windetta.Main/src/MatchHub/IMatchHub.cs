using Windetta.Main.MatchHub.Strategies;
using Windetta.Main.Rooms;

namespace Windetta.Main.MatchHub;

public interface IMatchHub : IHubDisposeListener, IHubReadyListener, IDisposable
{
    event EventHandler Updated;
    event EventHandler Disposed;
    event EventHandler Ready;

    void Add(RoomMember member, Guid roomId);
    void Remove(Guid memberId);
}