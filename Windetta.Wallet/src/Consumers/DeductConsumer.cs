using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Consumers;

public class DeductConsumer : IConsumer<IDeductBalance>
{
    private readonly IUserWalletService _walletService;

    public DeductConsumer(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task Consume(ConsumeContext<IDeductBalance> context)
    {
        var userId = context.Message.UserId;
        var value = context.Message.Amount;

        await _walletService.DeductAsync(new(userId, value)
        {
            OperationId = context.Message.CorrelationId
        });
    }
}

public class DeductConsumerDefinition : ConsumerDefinition<DeductConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<DeductConsumer> consumerConfigurator, IRegistrationContext context)
    {
        consumerConfigurator.UseScheduledRedelivery(r =>
        {
            r.Intervals(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(4));
        });

        consumerConfigurator.UseMessageRetry(r =>
        {
            r.Interval(retryCount: 3, interval: TimeSpan.FromSeconds(10));
        });
    }
}