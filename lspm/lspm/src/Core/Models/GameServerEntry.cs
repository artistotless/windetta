namespace LSPM.Core.Models;

public class GameServerEntry : GameServerInfo
{
    public Guid GameId { get; set; }
    public DateTimeOffset LaunchTime { get; set; }
    public DateTimeOffset LastPing { get; private set; }
    public IReadOnlyCollection<Guid> Matches => _matches.AsReadOnly();

    private List<Guid> _matches = new();
    private object _lockObj = new object();

    public GameServerEntry()
    {
        LastPing = DateTimeOffset.UtcNow;
        LaunchTime = DateTimeOffset.UtcNow;
    }

    public void AddMatchWithLock(Guid matchId)
    {
        lock (_lockObj)
        {
            _matches.Add(matchId);
        }
    }

    public void RemoveMatchWithLock(Guid matchId)
    {
        lock (_lockObj)
        {
            _matches.Remove(matchId);
        }
    }

    public void UpdatePingWithLock()
    {
        lock (_lockObj)
        {
            LastPing = DateTimeOffset.UtcNow;
        }
    }

    public object GetLock()
    {
        return _lockObj;
    }
}