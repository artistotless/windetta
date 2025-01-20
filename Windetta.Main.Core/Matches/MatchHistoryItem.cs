using LSPM.Models;
using Windetta.Contracts;

namespace Windetta.Main.Core.Matches;

/// <summary>
/// Represents a match that is preserved in history
/// </summary>
public class MatchHistoryItem
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
    public FundsInfo Bet { get; set; }

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

    /// <summary>
    /// Date and time when the match started
    /// </summary>
    public DateTimeOffset StartDate { get; set; }

    /// <summary>
    /// Date and time when the match ended
    /// </summary>
    public DateTimeOffset EndDate { get; set; }
}