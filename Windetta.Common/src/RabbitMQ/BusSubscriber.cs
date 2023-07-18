using Microsoft.AspNetCore.Builder;
using System.Windows.Input;
using Windetta.Common.Messages;

namespace Windetta.Common.RabbitMQ;

public class BusSubscriber : IBusSubscriber
{
    private IApplicationBuilder app;

    public BusSubscriber(IApplicationBuilder app)
    {
        this.app = app;
    }

    //public BusSubscriber()

    public IBusSubscriber SubscribeCommand<TCommand>() where TCommand : ICommand
    {
        throw new NotImplementedException();
    }

    public IBusSubscriber SubscribeEvent<TEvent>(string? @namespace = null) where TEvent : IEvent
    {
        throw new NotImplementedException();
    }
}
