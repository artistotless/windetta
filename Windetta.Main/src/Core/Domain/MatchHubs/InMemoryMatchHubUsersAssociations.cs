using System.Collections.Concurrent;

namespace Windetta.Main.Core.Domain.MatchHubs;

public sealed class InMemoryMatchHubUsersAssociations : IMatchHubUsersAssociations
{
    private readonly ConcurrentDictionary<Guid, Guid> _store;

    public InMemoryMatchHubUsersAssociations()
    {
        _store = new();
    }

    public Guid? GetHubId(Guid userId)
    {
        Guid? result = null;

        if (_store.TryGetValue(userId, out Guid value))
            result = value;

        return result;
    }

    public void Set(Guid hubId, Guid userId)
    {
        _store.TryAdd(userId, hubId);
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
