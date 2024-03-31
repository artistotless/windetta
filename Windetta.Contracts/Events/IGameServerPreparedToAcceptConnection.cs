using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IGameServerPreparedToAcceptConnection : CorrelatedBy<Guid>, IEvent
{
    public Uri Endpoint { get; set; }
    public Dictionary<Guid, string> Tickets { get; set; }
    public DateTime TimeStamp { get; set; }
}