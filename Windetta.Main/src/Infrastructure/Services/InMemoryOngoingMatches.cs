using System.Collections.Concurrent;
using Windetta.Common.Types;
using Windetta.Main.Core.Matches;

public class InMemoryOngoingMatches : IOngoingMatches
{
    private readonly ConcurrentDictionary<Guid, OngoingMatch> _matches;

    public InMemoryOngoingMatches()
    {
        _matches = new();
    }

    public Task SetAsync(OngoingMatch ongoingMatch, Guid playerId)
    {
        _matches[playerId] = ongoingMatch;

        return Task.CompletedTask;
    }

    public Task SetRangeAsync(IEnumerable<(OngoingMatch ongoingMatch, Guid playerId)> values)
    {
        var tasks = values.Select(x => SetAsync(x.ongoingMatch, x.playerId));

        return Task.WhenAll(tasks);
    }

    Task<OngoingMatch> IOngoingMatches.GetAsync(Guid playerId)
    {
        if (!_matches.TryGetValue(playerId, out OngoingMatch ongoingMatch))
            throw new WindettaException("Ongoing match is not found");

        return Task.FromResult(ongoingMatch);
    }
}
