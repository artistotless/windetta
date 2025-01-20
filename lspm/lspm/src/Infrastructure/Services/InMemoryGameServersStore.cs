using LSPM.Core.Interfaces;
using LSPM.Core.Models;
using System.Collections.Concurrent;

namespace LSPM.Infrastructure.Services;

public class InMemoryGameServersStore : IGameServersStore
{
    private readonly ConcurrentDictionary<Guid, GameServerEntry> _entries;
    private readonly IGameServersOptions _optionsStore;

    public InMemoryGameServersStore(IGameServersOptions optionsStore)
    {
        _entries = new ConcurrentDictionary<Guid, GameServerEntry>();
        _optionsStore = optionsStore;
    }

    public void Add(GameServerEntry server)
    {
        _entries.TryAdd(server.InstanceId, server);
    }

    public IEnumerable<GameServerEntry> GetAll()
    {
        return _entries.Values;
    }

    public IEnumerator<KeyValuePair<Guid, GameServerEntry>> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    public bool AnyLoadFreeServer(Guid gameId)
    {
        var options = _optionsStore.Get(gameId);

        var copy = _entries.ToArray();
        return copy.Any(e => e.Value.Matches.Count() < options.MaxMatchesOnInstance && e.Value.GameId == gameId);
    }


    public GameServerEntry? GetLoadFreeServer(Guid gameId)
    {
        var options = _optionsStore.Get(gameId);

        var copy = _entries.ToArray();
        return copy.Where(e => e.Value.Matches.Count() < options.MaxMatchesOnInstance && e.Value.GameId == gameId)
            .FirstOrDefault().Value;
    }

    public int Count(Guid gameId)
    {
        var copy = _entries.ToArray();
        return copy.Count(e => e.Value.GameId == gameId);
    }

    public GameServerEntry? Find(Guid instanceId)
    {
        return _entries.TryGetValue(instanceId, out var entry) ? entry : null;
    }

    public void Remove(Guid instanceId)
    {
        _entries.TryRemove(instanceId, out _);
    }
}
