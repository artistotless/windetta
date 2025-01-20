using LSPM.Models;

namespace LSPM.Infrastructure.Services;

public static class DelayedMatchesQueue
{
    /// <summary>
    /// Contains matches that cannot be created right now
    /// These matches will be created as soon as the target game servers are in the status IsReady = true
    /// </summary>
    private static Dictionary<Guid, Queue<MatchInitializationData>> _delayedMatches = new();

    /// <summary>
    /// Get delayed matches for a specific game server
    /// </summary>
    /// <param name="gameServerId">Game server identifier</param>
    /// <returns>Queue of delayed matches</returns>
    public static Queue<MatchInitializationData>? GetDelayedMatches(Guid gameServerId)
        => _delayedMatches.GetValueOrDefault(gameServerId);

    /// <summary>
    /// Add match creation to the queue
    /// </summary>
    /// <param name="gameServerId">Game server identifier</param>
    /// <param name="initData">Data about the match to be created</param>
    /// <exception cref="Exception"></exception>
    public static void AddDelayedMatch(Guid gameServerId, MatchInitializationData initData)
    {
        _delayedMatches.TryAdd(gameServerId, new());

        if (_delayedMatches.TryGetValue(gameServerId, out var queue))
            queue.Enqueue(initData);
        else
            throw new Exception("No queue found in delayedMatches");
    }
}
