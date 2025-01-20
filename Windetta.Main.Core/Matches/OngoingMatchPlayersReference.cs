using LSPM.Models;

namespace Windetta.Main.Core.Matches;

/// <summary>
/// Provides information about which players are in the match
/// </summary>
public sealed class OngoingMatchPlayersReference
{
    /// <summary>
    /// Match ID
    /// </summary>
    public Guid MatchId { get; init; }

    /// <summary>
    /// List of players participating in the match
    /// </summary>
    public IEnumerable<Player> Players { get; init; }


    public OngoingMatchPlayersReference(Guid id, IEnumerable<Player> players)
    {
        MatchId = id;
        Players = players;
    }
}