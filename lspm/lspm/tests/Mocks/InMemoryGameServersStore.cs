using LSPM.Core.Interfaces;
using LSPM.Core.Models;

namespace LspmTests.Mocks;

public class InMemoryGameServersStore : IGameServersStore
{
    private readonly List<GameServerEntry> _entries;
    private readonly IGameServersOptions _optionsStore;

    public InMemoryGameServersStore(IGameServersOptions optionsStore)
    {
        _entries = new List<GameServerEntry>();
        _optionsStore = optionsStore;
    }

    public void Add(GameServerEntry server)
    {
        _entries.Add(server);
    }

    public bool AnyLoadFreeServer(Guid gameId)
    {
        var options = _optionsStore.Get(gameId);

        return _entries
            .Any(e => e.Matches.Count() < options.MaxMatchesOnInstance && e.GameId == gameId);
    }

    public int Count(Guid gameId)
    {
        return _entries.Count(e => e.GameId == gameId);
    }

    public GameServerEntry? Find(Guid instanceId)
    {
        return _entries.Find(x => x.InstanceId == instanceId);
    }

    public IEnumerable<GameServerEntry> GetAll()
    {
        return _entries;
    }

    public GameServerEntry? GetLoadFreeServer(Guid gameId)
    {
        var options = _optionsStore.Get(gameId);

        return _entries
            .Where(e => e.Matches.Count() < options.MaxMatchesOnInstance && e.GameId == gameId)
            .FirstOrDefault();
    }

    public void Remove(Guid instanceId)
    {
        _entries.RemoveAll(e => e.InstanceId == instanceId);
    }

    public void Update(GameServerEntry server)
    {
        Remove(server!.InstanceId);
        Add(server);
    }
}
