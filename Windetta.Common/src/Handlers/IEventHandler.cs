using Windetta.Common.Messages;
using Windetta.Common.RabbitMQ;

namespace Windetta.Common.Handlers;

public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    Task HandleAsync(TEvent @event, ICorrelationContext context);
}
