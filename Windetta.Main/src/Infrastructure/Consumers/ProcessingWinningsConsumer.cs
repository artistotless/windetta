using MassTransit;
using Windetta.Contracts.Commands;

namespace Windetta.Main.Infrastructure.Consumers;

public class ProcessingWinningsConsumer : IConsumer<ICreateWinningsFlowRequested>
{
    public async Task Consume(ConsumeContext<ICreateWinningsFlowRequested> context)
    {
        // TODO: implement it
        throw new NotImplementedException();

        //await context.Publish<IWinningsProcessed>(new
        //{
        //    context.Message.CorrelationId,
        //});
    }
}
