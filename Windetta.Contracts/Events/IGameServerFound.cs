using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

/// <summary>
/// The event is called when the search for a game server has successfully completed
/// </summary>
public interface IGameServerFound : CorrelatedBy<Guid>, IEvent
{
    /// <summary>
    /// Game server instance identifier
    /// </summary>
    public Guid GameServerId { get; set; }

    /// <summary>
    /// Ip address of the LSPM where the game server was found
    /// </summary>
    public string LspmIp { get; set; }
}