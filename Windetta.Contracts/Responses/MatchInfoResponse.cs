using LSPM.Models;

namespace Windetta.Contracts.Responses;

public sealed class MatchInfoResponse : ServiceResponse
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
    public FundsInfo Bet { get; init; }

    /// <summary>
    /// When the match was created
    /// </summary>
    public DateTimeOffset Created { get; init; }

    /// <summary>
    /// GameServer endpoint
    /// </summary>
    public Uri GameServerEndpoint { get; set; }
}
