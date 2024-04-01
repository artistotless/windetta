using Windetta.Common.Types;

namespace Windetta.Main.Core.Matches;

/// <summary>
/// Service storing match history
/// </summary>
public interface IMatchesHistory
{
    /// <summary>
    /// Gets the list of matches for a certain period
    /// </summary>
    /// <param name="dateRange">Period</param>
    /// <param name="playerId">Only matches in which this player participated</param>
    /// <param name="gameId">Only matches for this game</param>
    /// <returns>List of matches for a certain period</returns>
    Task<IEnumerable<Match>> GetAsync(DateRange dateRange, Guid? playerId = null, Guid? gameId = null);
}