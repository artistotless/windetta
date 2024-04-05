using Windetta.Contracts;
using Windetta.Contracts.Responses;

namespace LSPM.Models;

public sealed class GameServerResponse : ServiceResponse
{
    public Uri GameServerEndpoint { get; set; }
    public Guid GameServerId { get; set; }
}

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
    public DateTime Created { get; init; }
}
