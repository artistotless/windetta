using RawRabbit;
using RawRabbit.Enrichers.MessageContext;
using System.Windows.Input;
using Windetta.Common.Messages;

namespace Windetta.Common.RabbitMQ;

public class BusPublisher : IBusPublisher
{
    private readonly IBusClient _busClient;

    public BusPublisher(IBusClient busClient) => _busClient = busClient;

    public async Task PublishAsync(IEvent @event, ICorrelationContext context)
    {
        await _busClient.PublishAsync(@event, ctx => ctx.UseMessageContext(context));
    }

    public async Task SendAsync(ICommand command, ICorrelationContext context)
    {
        await _busClient.PublishAsync(command, ctx => ctx.UseMessageContext(context));
    }
}
