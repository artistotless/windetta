using System.Collections.Concurrent;

namespace Windetta.Main.Core.Lobbies;

public sealed class InMemoryLobbyUserMaps : IUserLobbyMaps
{
    private readonly ConcurrentDictionary<Guid, UserLobbyMapEntry> _store;

    public InMemoryLobbyUserMaps()
    {
        _store = new();
    }

    public UserLobbyMapEntry? Get(Guid userId)
    {
        if (_store.TryGetValue(userId, out UserLobbyMapEntry value))
            return value;

        return null;
    }

    public void Set(UserLobbyMapEntry entry)
    {
        _store.TryAdd(entry.UserId, entry);
    }

    public void Unset(Guid userId)
    {
        _store.TryRemove(userId, out _);
    }

    public void Unset(IEnumerable<Guid> userIds)
    {
        foreach (var userId in userIds)
            Unset(userId);
    }
}
