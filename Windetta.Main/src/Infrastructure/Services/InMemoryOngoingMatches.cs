using System.Collections.Concurrent;
using Windetta.Common.Types;
using Windetta.Main.Core.Matches;

namespace Windetta.Main.Infrastructure.Services;

/// <inheritdoc cref="IOngoingMatches"/>
public class InMemoryOngoingMatches : IOngoingMatches
{
    private readonly ConcurrentDictionary<Guid, OngoingMatchPlayersReference> _matches;
    private readonly ConcurrentDictionary<Guid, Guid> _references;

    public InMemoryOngoingMatches()
    {
        _matches = new();
        _references = new();
    }

    public Task AddAsync(OngoingMatchPlayersReference match)
    {
        _matches.TryAdd(match.MatchId, match);

        SetReferenceAsync(match.Players.Select(p => p.Id), match.MatchId);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(Guid matchId)
    {
        _matches.TryRemove(matchId, out var removedMatch);

        if (removedMatch is not null)
            UnSetReferenceAsync(removedMatch.Players.Select(p => p.Id));

        return Task.CompletedTask;
    }

    public Task<Guid> GetMatchIdOfPlayerAsync(Guid playerId)
    {
        if (!_references.TryGetValue(playerId, out Guid ongoingMatchId))
            throw new WindettaException("Ongoing match is not found");

        return Task.FromResult(ongoingMatchId);
    }

    public Task<IEnumerable<Guid>> GetAllIdsAsync()
        => Task.FromResult((IEnumerable<Guid>)_matches.Keys);

    #region private methods
    private Task SetReferenceAsync(Guid ongoingMatchId, Guid playerId)
    {
        _references[playerId] = ongoingMatchId;

        return Task.CompletedTask;
    }

    private Task SetReferenceAsync(IEnumerable<Guid> playersIds, Guid ongoingMatchId)
    {
        var tasks = playersIds.Select(x => SetReferenceAsync(ongoingMatchId, x));

        return Task.WhenAll(tasks);
    }

    private Task UnSetReferenceAsync(Guid playerId)
    {
        _references.TryRemove(playerId, out _);

        return Task.CompletedTask;
    }

    private Task UnSetReferenceAsync(IEnumerable<Guid> playersIds)
    {
        var tasks = playersIds.Select(UnSetReferenceAsync);

        return Task.WhenAll(tasks);
    }
    #endregion
}
