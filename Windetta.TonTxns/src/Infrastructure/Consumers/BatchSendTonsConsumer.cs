using MassTransit;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts.Commands;

namespace Windetta.TonTxns.Infrastructure.Consumers;

public class BatchSendTonsConsumer : IConsumer<Batch<ISendTons>>
{
    public async Task Consume(ConsumeContext<Batch<ISendTons>> context)
    {
        var endpoint = MyEndpointNameFormatter
        .CommandUri<IPackedSendTons>(Svc.TonTxns);

        await context.Send<IPackedSendTons>(endpoint, new
        {
            CorrelationId = Guid.NewGuid(),
            Sends = context.Message.Select(x => x.Message)
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
