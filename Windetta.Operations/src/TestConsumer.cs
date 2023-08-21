using MassTransit;
using Windetta.Contracts.Events;

namespace Windetta.Operations;

public class TestConsumer : IConsumer<IUserCreated>
{
    public Task Consume(ConsumeContext<IUserCreated> context)
    {
        var msgid = context.MessageId;
        var val = context.Message;
        ;
        return Task.CompletedTask;
    }
}