using System.Collections.Concurrent;

namespace Windetta.Main.Core.Lobbies;

public sealed class InMemoryLobbyUsersAssociations : ILobbyUsersAssociations
{
    private readonly ConcurrentDictionary<Guid, Guid> _store;

    public InMemoryLobbyUsersAssociations()
    {
        _store = new();
    }

    public Guid? GetLobbyId(Guid userId)
    {
        Guid? result = null;

        if (_store.TryGetValue(userId, out Guid value))
            result = value;

        return result;
    }

    public void Set(Guid lobbyId, Guid userId)
    {
        _store.TryAdd(userId, lobbyId);
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
