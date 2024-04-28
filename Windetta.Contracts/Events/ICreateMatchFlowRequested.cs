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
    /// 
    /// </summary>
    public string LspmIp { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid GameServerId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Uri GameServerEndpoint { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyDictionary<string, string>? Properties { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<Player> Players { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid GameId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public FundsInfo Bet { get; set; }
}
