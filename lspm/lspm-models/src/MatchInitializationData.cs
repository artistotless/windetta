namespace LSPM.Models;

/// <summary>
/// Represents the initialization data for a match, including match ID, players, and optional properties.
/// </summary>
public sealed class MatchInitializationData
{
    /// <summary>
    /// Gets or sets the unique identifier for the match.
    /// </summary>
    public Guid MatchId { get; set; }

    /// <summary>
    /// Gets or sets the collection of players participating in the match.
    /// </summary>
    public IEnumerable<Player> Players { get; set; }

    /// <summary>
    /// Gets or sets additional properties related to the match, if any.
    /// Example: boardSize:5, gameMode:pvp
    /// </summary>
    public Dictionary<string, string>? Properties { get; set; }
}
