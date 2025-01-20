using Windetta.Common.Types;

namespace Windetta.Main.Core.Matches;

/// <summary>
/// Service that stores current matches of players
/// </summary>
public interface IOngoingMatches : ISingletonService
{
    /// <summary>
    /// Adds a match
    /// </summary>
    /// <param name="match">Ongoing match instance</param>
    public Task AddAsync(OngoingMatchPlayersReference match);

    /// <summary>
    /// Deletes the match
    /// </summary>
    /// <param name="matchId">Ongoing match ID</param>
    public Task RemoveAsync(Guid matchId);

    /// <summary>
    /// Gets the player's current match
    /// </summary>
    /// <param name="playerId">Player ID</param>
    /// <returns>Current match ID</returns>
    public Task<Guid> GetMatchIdOfPlayerAsync(Guid playerId);

    /// <summary>
    /// Gets the IDs of all matches in progress
    /// </summary>
    /// <returns>Collection of IDs</returns>
    public Task<IEnumerable<Guid>> GetAllIdsAsync();
}