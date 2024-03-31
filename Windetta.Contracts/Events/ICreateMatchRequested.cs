using LSPM.Models;
using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

/// <summary>
/// Request to LSPM to create a match
/// </summary>
public interface ICreateMatchRequested : CorrelatedBy<Guid>, IEvent, ILspmMessage
{
    /// <summary>
    /// 
    /// </summary>
    public Guid GameServerId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<Player> Players { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, string>? Properties { get; set; }
}