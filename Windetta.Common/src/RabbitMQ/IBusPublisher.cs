using System.Windows.Input;
using Windetta.Common.Messages;

namespace Windetta.Common.RabbitMQ;

public interface IBusPublisher
{
    Task SendAsync(ICommand command, ICorrelationContext context);
    Task PublishAsync(IEvent @event, ICorrelationContext context);
}
