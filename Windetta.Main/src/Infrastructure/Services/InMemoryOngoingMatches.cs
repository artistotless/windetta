using System.Collections.Concurrent;
using Windetta.Common.Types;
using Windetta.Main.Core.Matches;

namespace Windetta.Main.Infrastructure.Services;

/// <inheritdoc cref="IOngoingMatches"/>
public class InMemoryOngoingMatches : IOngoingMatches
{
    private readonly ConcurrentDictionary<Guid, Guid> _matches;

    public InMemoryOngoingMatches()
    {
        _matches = new();
    }

    public Task<IEnumerable<Guid>> GetAllAsync()
        => Task.FromResult(_matches.Values.AsEnumerable());

    Task<Guid> IOngoingMatches.GetAsync(Guid playerId)
    {
        if (!_matches.TryGetValue(playerId, out Guid ongoingMatchId))
            throw new WindettaException("Ongoing match is not found");

        return Task.FromResult(ongoingMatchId);
    }

    public Task SetAsync(Guid ongoingMatchId, Guid playerId)
    {
        _matches[playerId] = ongoingMatchId;

        return Task.CompletedTask;
    }

    public Task SetRangeAsync(IEnumerable<(Guid matchId, Guid playerId)> values)
    {
        var tasks = values.Select(x => SetAsync(x.matchId, x.playerId));

        return Task.WhenAll(tasks);
    }
}
