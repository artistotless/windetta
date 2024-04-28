using MassTransit;
using Windetta.Contracts.Commands;

namespace Windetta.Main.Infrastructure.Consumers;

public class ProcessingWinningsConsumer : IConsumer<IProcessWinnings>
{
    public async Task Consume(ConsumeContext<IProcessWinnings> context)
    {
        // TODO: implement it
        throw new NotImplementedException();

        //await context.Publish<IWinningsProcessed>(new
        //{
        //    context.Message.CorrelationId,
        //});
    }
}
