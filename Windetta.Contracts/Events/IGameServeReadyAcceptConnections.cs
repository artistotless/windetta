using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IGameServeReadyAcceptConnections : CorrelatedBy<Guid>, IEvent
{

}
