using Windetta.Common.Types;

namespace Windetta.Main.Core.Matches;

/// <summary>
/// Service that stores current matches of players
/// </summary>
public interface IOngoingMatches : ISingletonService
{
    /// <summary>
    /// Sets the current match for the player
    /// </summary>
    /// <param name="ongoingMatch">Current match</param>
    /// <param name="playerId">Player ID</param>
    public Task SetAsync(Guid matchId, Guid playerId);

    /// <summary>
    /// Sets the current matches for the players
    /// </summary>
    /// <param name="values">Current matches</param>
    public Task SetRangeAsync(Guid matchId, IEnumerable<Guid> playersIds);

    /// <summary>
    /// Gets the player's current match
    /// </summary>
    /// <param name="playerId">Player ID</param>
    /// <returns>Current match ID</returns>
    public Task<Guid> GetAsync(Guid playerId);

    /// <summary>
    /// gets the IDs of all matches in progress
    /// </summary>
    /// <returns>Collection of IDs</returns>
    public Task<IEnumerable<Guid>> GetAllAsync();
}