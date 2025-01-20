using LSPM.Models;
using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

/// <summary>
/// Request for MatchFlow Saga Creation
/// </summary>
public interface ICreateMatchFlowRequested : CorrelatedBy<Guid>, IEvent
{
    /// <summary>
    /// IP address of the local manager of game servers
    /// </summary>
    public string LspmIp { get; set; }

    /// <summary>
    /// Game server instance identifier
    /// </summary>
    public Guid GameServerId { get; set; }

    /// <summary>
    /// Game server instance endpoint
    /// </summary>
    public Uri GameServerEndpoint { get; set; }

    /// <summary>
    /// Optional match properties
    /// </summary>
    public IReadOnlyDictionary<string, string>? Properties { get; set; }

    /// <summary>
    /// Players
    /// </summary>
    public IEnumerable<Player> Players { get; set; }

    /// <summary>
    /// Unique gaame identifier
    /// </summary>
    public Guid GameId { get; set; }

    /// <summary>
    /// The bet the players are going to gamble on
    /// </summary>
    public FundsInfo Bet { get; set; }
}
