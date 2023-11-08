using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Events;

public interface ISendTonsCompleted : CorrelatedBy<Guid>, IEvent
{

}
