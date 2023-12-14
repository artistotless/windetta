using System.Collections.ObjectModel;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.MatchHubs.Plugins;
using Windetta.Main.Core.Rooms;

namespace Windetta.Main.Core.MatchHubs;

public class MatchHub : IMatchHub
{
    public Guid Id { get; init; }
    public Guid GameId { get; init; }
    public bool IsPublic { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int MembersCount { get; private set; }
    public Bet Bet { get; init; }
    public Guid InitiatorId { get; init; }
    public GameConfiguration Configuration { get; init; }
    public IEnumerable<string>? JoinFilters => _joinFilters?
        .Select(filter => filter.GetType().Name);
    public string? AutoReadyStrategy => _readyStrategy?.GetType().Name;
    public string? AutoDisposeStrategy => _disposeStrategy?.GetType().Name;
    public IEnumerable<Room> Rooms => (IsDisposed ? null : _rooms.Values)!;
    public MatchHubState State { get; set; }
    public bool IsDisposed { get; private set; }

    //Events
    public event EventHandler? Updated;
    public event EventHandler? Disposed;
    public event EventHandler? Ready;

    // Plugins
    private IAutoReadyStrategy? _readyStrategy;
    private IAutoDisposeStrategy? _disposeStrategy;
    private IEnumerable<IJoinFilter>? _joinFilters;

    private IReadOnlyDictionary<Guid, Room> _rooms;


    public MatchHub(MatchHubOptions options, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        State = MatchHubState.Awaiting;
        InitiatorId = options.InitiatorId;
        CreatedAt = DateTimeOffset.UtcNow;
        IsPublic = !options.Private;
        GameId = options.GameId;
        Configuration = options.GameConfiguration;
        Bet = options.Bet;
        _rooms = CreateRooms();

        _joinFilters = options.JoinFilters;
    }

    public void SetAutoReadyStrategy(IAutoReadyStrategy strategy)
    {
        if (_readyStrategy is not null)
            _readyStrategy.Dispose();

        _readyStrategy = strategy;
        _readyStrategy.Start(this);

        OnUpdated();
    }

    public void SetDisposeStrategy(IAutoDisposeStrategy strategy)
    {
        if (_disposeStrategy is not null)
            _disposeStrategy.Dispose();

        _disposeStrategy = strategy;
        _disposeStrategy.Start(this);

        OnUpdated();
    }

    public void Add(RoomMember member, Guid roomId)
    {
        if (!_rooms.TryGetValue(roomId, out var room))
            return;

        member.Join(room);

        MembersCount++;

        OnUpdated();
    }

    public void Remove(Guid memberId)
    {
        var room = _rooms.Values
            .Where(x => x.Members
            .Any(x => x.Id == memberId))
            .FirstOrDefault();

        if (room is null)
            return;

        var member = room.Members
            .First(x => x.Id == memberId);

        member.LeaveRoom();

        MembersCount--;

        OnUpdated();
    }

    public void Dispose()
    {
        if (IsDisposed)
            return;

        _readyStrategy?.Dispose();
        _disposeStrategy?.Dispose();

        _readyStrategy = null;
        _disposeStrategy = null;

        _rooms = null;

        MembersCount = 0;

        Disposed?.Invoke(this, null);

        IsDisposed = true;
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

    public IEnumerable<IJoinFilter>? GetJoinFilters()
    {
        return _joinFilters;
    }

    private void OnUpdated()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
        Updated?.Invoke(this, null);
    }

    private IReadOnlyDictionary<Guid, Room> CreateRooms()
    {
        var maxTeams = Math.Max(1, Configuration.MaxTeams);

        IEnumerable<KeyValuePair<Guid, Room>> BuildKeyValuePairs()
        {
            for (uint i = 0; i < maxTeams; i++)
            {
                var roomId = Guid.NewGuid();
                var room = new Room(roomId, Configuration.MaxPlayers);

                yield return new KeyValuePair<Guid, Room>(roomId, room);
            }
        }

        return new ReadOnlyDictionary<Guid, Room>(
            new Dictionary<Guid, Room>(BuildKeyValuePairs()));
    }
}