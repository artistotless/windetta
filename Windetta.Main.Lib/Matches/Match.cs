using LSPM.Models;
using Windetta.Main.Core.Lobbies;

namespace Windetta.Main.Core.Matches;

/// <summary>
/// Represents a match that is preserved in history
/// </summary>
public class Match
{
    /// <summary>
    /// Match ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Game ID
    /// </summary>
    public Guid GameId { get; set; }

    /// <summary>
    /// Rate amount
    /// </summary>
    public Bet Bet { get; set; }

    /// <summary>
    /// State of the match (Canceled, Completed)
    /// </summary>
    public MatchState State { get; set; }

    /// <summary>
    /// Players who took part in the match
    /// </summary>
    public IEnumerable<Player> Players { get; set; }

    /// <summary>
    /// List of winners in the match
    /// </summary>
    public IEnumerable<Guid>? Winners { get; set; }
}