using MassTransit;
using Windetta.Contracts.Events;

namespace Windetta.Operations.Sagas;

public class TestConsumer : IConsumer<IUserCreated>
{
    public async Task Consume(ConsumeContext<IUserCreated> context)
    {
        ;
        await Task.CompletedTask;
    }
}
