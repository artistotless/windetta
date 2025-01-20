namespace LSPM.Models;

/// <summary>
/// Represents information about the winners of a match or game.
/// </summary>
public sealed class WinnersInfo
{
    /// <summary>
    /// Gets or sets the collection of unique identifiers for the winners.
    /// </summary>
    public IEnumerable<Guid> Winners { get; set; }
}