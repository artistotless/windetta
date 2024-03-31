using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

/// <summary>
/// Request to LSPM to start or retrieve info about already running server
/// </summary>
public interface IGameServerRequested : CorrelatedBy<Guid>, IEvent, ILspmMessage
{
    /// <summary>
    /// What kind of game do you need a game server for
    /// </summary>
    public Guid GameId { get; set; }

    /// <summary>
    /// The moment in time when the request was generated. 
    /// Needed to determine if the message has expired
    /// </summary>
    public DateTimeOffset TimeStamp { get; set; }
}