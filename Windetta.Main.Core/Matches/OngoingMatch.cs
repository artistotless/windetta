using LSPM.Models;
using Windetta.Main.Core.Lobbies;

namespace Windetta.Main.Core.Matches;

/// <summary>
/// Presents information about the match that is going on right now
/// </summary>
public sealed class OngoingMatch
{
    /// <summary>
    /// Match ID
    /// </summary>
    public Guid MatchId { get; init; }

    /// <summary>
    /// What game the match is on
    /// </summary>
    public Guid GameId { get; init; }

    /// <summary>
    /// List of players participating in the match
    /// </summary>
    public IEnumerable<Player> Players { get; init; }

    /// <summary>
    /// How much each player bet
    /// </summary>
    public Bet Bet { get; init; }

    /// <summary>
    /// When the match was created
    /// </summary>
    public DateTime Created { get; init; }
}