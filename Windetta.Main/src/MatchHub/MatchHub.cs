using System.Collections.ObjectModel;
using Windetta.Main.Games;
using Windetta.Main.MatchHub.Strategies;
using Windetta.Main.Rooms;

namespace Windetta.Main.MatchHub;

public class MatchHub : IMatchHub
{
    public bool IsPublic { get; init; }
    public DateTimeOffset Created { get; init; }
    public Bet Bet { get; init; }
    public GameConfiguration Configuration { get; init; }
    public IEnumerable<Room> Rooms => _rooms.Values;

    //Events
    public event EventHandler Updated;
    public event EventHandler Disposed;
    public event EventHandler Ready;

    private AutoReadyStrategy _readyStrategy;
    private AutoDisposeStrategy _disposeStrategy;

    private IReadOnlyDictionary<Guid, Room> _rooms;

    public MatchHub(MatchHubOptions options)
    {
        IsPublic = !options.Private;
        Configuration = options.GameConfiguration;
        Bet = options.Bet;
        _rooms = CreateRooms();

        if (options.AutoReadyStrategy is not null)
            SetAutoReadyStrategy(options.AutoReadyStrategy);

        if (options.AutoDisposeStrategy is not null)
            SetDisposeStrategy(options.AutoDisposeStrategy);
    }

    public void SetAutoReadyStrategy(AutoReadyStrategy strategy)
    {
        if (_readyStrategy is not null)
            _readyStrategy.Dispose();

        _readyStrategy = strategy;
        _readyStrategy.Start(this);
    }

    public void SetDisposeStrategy(AutoDisposeStrategy strategy)
    {
        if (_disposeStrategy is not null)
            _disposeStrategy.Dispose();

        _disposeStrategy = strategy;
        _disposeStrategy.Start(this);
    }

    public void Add(RoomMember member, Guid roomId)
    {
        if (_rooms.TryGetValue(roomId, out Room room))
        {
            member.Join(room);
        }

        Updated?.Invoke(this, null);
    }

    public void Remove(Guid memberId)
    {
        var room = _rooms.Values
            .Where(x => x.Members.Any(x => x.Id == memberId))
            .FirstOrDefault();

        if (room is null)
            return;

        var member = room.Members.First(x => x.Id == memberId);
        member.LeaveRoom();

        Updated?.Invoke(this, null);
    }

    public void Dispose()
    {
        _readyStrategy?.Dispose();
        _disposeStrategy?.Dispose();

        _readyStrategy = null;
        _disposeStrategy = null;
        _rooms = null;

        Disposed?.Invoke(this, null);
    }

    void IHubReadyListener.OnHubAutoReady()
    {
        Ready?.Invoke(this, null);
    }

    void IHubDisposeListener.OnHubAutoDisposed()
    {
        Dispose();
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