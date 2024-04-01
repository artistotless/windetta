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
    public Task SetAsync(OngoingMatch ongoingMatch, Guid playerId);

    /// <summary>
    /// Sets the current matches for the players
    /// </summary>
    /// <param name="values">Current matches</param>
    public Task SetRangeAsync(IEnumerable<(OngoingMatch ongoingMatch, Guid playerId)> values);

    /// <summary>
    /// Gets the player's current match
    /// </summary>
    /// <param name="playerId">Player ID</param>
    /// <returns>Current match ID with ticket</returns>
    public Task<OngoingMatch> GetAsync(Guid playerId);

    /// <summary>
    /// gets the IDs of all matches in progress
    /// </summary>
    /// <returns>Collection of IDs</returns>
    public Task<IEnumerable<Guid>> GetAllAsync();
}