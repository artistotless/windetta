using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface ISendTonsCompleted : CorrelatedBy<Guid>, IEvent
{

}
