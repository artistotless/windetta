using System.Collections.ObjectModel;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Core.MatchHubs.Plugins;
using Windetta.Main.Core.Rooms;

namespace Windetta.MainTests.Mocks;

public class ProxyMatchHub : IMatchHub
{
    public Guid Id => _original.Id;
    public Guid GameId { get { return _original.Id; } init { } }
    public bool IsPublic { get { return _original.IsPublic; } init { } }
    public DateTimeOffset CreatedAt { get { return _original.CreatedAt; } init { } }
    public DateTimeOffset UpdatedAt { get { return _original.UpdatedAt; } set { _original.UpdatedAt = value; } }
    public int MembersCount { get { return _original.MembersCount; } private set { } }
    public Bet Bet { get { return _original.Bet; } init { } }
    public Guid InitiatorId { get { return _original.InitiatorId; } init { } }
    public GameConfiguration Configuration { get { return _original.Configuration; } init { } }

    public IEnumerable<string>? JoinFilters => _original.JoinFilters;

    public string? AutoReadyStrategy => _original.AutoReadyStrategy;
    public string? AutoDisposeStrategy => _original.AutoDisposeStrategy;
    public IEnumerable<Room> Rooms => _original.Rooms;
    public MatchHubState State { get { return _original.State; } set { _original.State = value; } }
    public bool IsDisposed { get { return _original.IsDisposed; } private set { } }

    //Events
    public event EventHandler? Updated;
    public event EventHandler? Disposed;
    public event EventHandler? Ready;

    // Plugins
    private readonly MatchHub _original;

    public ProxyMatchHub(MatchHubOptions options, Guid? id = null)
    {
        _original = new MatchHub(options, id);
    }

    public virtual void SetAutoReadyStrategy(IAutoReadyStrategy strategy)
    {
        _original.SetAutoReadyStrategy(strategy);
    }

    public virtual void SetDisposeStrategy(IAutoDisposeStrategy strategy)
    {
        _original.SetDisposeStrategy(strategy);
    }

    public virtual void Add(RoomMember member, ushort roomIndex)
    {
        _original.Add(member, roomIndex);
    }

    public virtual void Remove(Guid memberId)
    {
        _original.Remove(memberId);
    }

    public virtual void Dispose()
    {
        _original.Dispose();
    }

    void IHubReadyListener.OnHubAutoReady()
    {
        State = MatchHubState.Ready;
        Ready?.Invoke(this, null);
    }

    void IHubDisposeListener.OnHubAutoDisposed()
    {
        Dispose();
    }

    public virtual IEnumerable<IJoinFilter>? GetJoinFilters()
    {
        return _original.GetJoinFilters();
    }

    private IReadOnlyDictionary<ushort, Room> CreateRooms()
    {
        var maxTeams = Math.Max(1, Configuration.MaxTeams);

        IEnumerable<KeyValuePair<ushort, Room>> BuildKeyValuePairs()
        {
            for (ushort i = 0; i < maxTeams; i++)
            {
                var room = new Room(i, Configuration.MaxPlayers);

                yield return new KeyValuePair<ushort, Room>(i, room);
            }
        }

        return new ReadOnlyDictionary<ushort, Room>(
            new Dictionary<ushort, Room>(BuildKeyValuePairs()));
    }
}