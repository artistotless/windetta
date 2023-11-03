using MassTransit;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts.Commands;

namespace Windetta.TonTxns.Consumers;

public class BatchSendTonsConsumer : IConsumer<Batch<ISendTons>>
{
    public async Task Consume(ConsumeContext<Batch<ISendTons>> context)
    {
        var endpoint = new MyEndpointNameFormatter(Svc.TonTxns)
        .CommandUri<IPackedSendTons>();

        await context.Send<IPackedSendTons>(endpoint, new
        {
            Transfers = context.Message.Select(x => x.Message)
        });
    }
}

public class BatchSendTonsConsumerDefinition : ConsumerDefinition<BatchSendTonsConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<BatchSendTonsConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.PrefetchCount = 100;
        consumerConfigurator.Options<BatchOptions>(options => options
        .SetMessageLimit(100)
        .SetConcurrencyLimit(4)
        .SetTimeLimit(TimeSpan.FromSeconds(10)));

        consumerConfigurator.UseMessageRetry(r =>
        {
            r.Interval(retryCount: 5, interval: TimeSpan.FromSeconds(10));
        });
    }
}
