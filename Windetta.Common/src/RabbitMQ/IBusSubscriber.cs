using System.Windows.Input;
using Windetta.Common.Messages;

namespace Windetta.Common.RabbitMQ;

public interface IBusSubscriber
{
    IBusSubscriber SubscribeCommand<TCommand>() where TCommand : ICommand;
    IBusSubscriber SubscribeEvent<TEvent>(string? @namespace = null) where TEvent : IEvent;
}
