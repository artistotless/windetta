using System.Windows.Input;
using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Common.RabbitMQ;

public interface IBusSubscriber
{
    IBusSubscriber SubscribeCommand<TCommand>(Func<TCommand, WindettaException, IRejectedEvent>? onError = null)
        where TCommand : ICommand;
    IBusSubscriber SubscribeEvent<TEvent>(string? @namespace = null, Func<TEvent, WindettaException, IRejectedEvent>? onError = null)
        where TEvent : IEvent;
}
