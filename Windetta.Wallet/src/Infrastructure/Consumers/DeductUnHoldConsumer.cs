using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Application.Services;
using Windetta.Wallet.Exceptions;

namespace Windetta.Wallet.Infrastructure.Consumers;

/// <summary>
/// Burns off funds on hold
/// </summary>
public class DeductUnHoldConsumer : IConsumer<IDeductUnHoldBalance>
{
    private readonly IUserWalletService _walletService;

    public DeductUnHoldConsumer(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task Consume(ConsumeContext<IDeductUnHoldBalance> context)
    {
        var arg = new DeductUnHoldArgument(context.Message.Data, context.Message.Type);

        await _walletService.DeductUnHoldAsync(arg);

        await context.Publish<IBalanceDeductedUnHeld>(new
        {
            context.Message.CorrelationId
        });
    }
}

public class DeductUnHoldConsumerDefinition : ConsumerDefinition<DeductConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<DeductConsumer> consumerConfigurator, IRegistrationContext context)
    {
        consumerConfigurator.UseInMemoryOutbox(context);
        consumerConfigurator.UseScheduledRedelivery(r =>
        {
            r.Ignore<WalletException>();
            r.Intervals(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(4));
        });

        consumerConfigurator.UseMessageRetry(r =>
        {
            r.Ignore<WalletException>();
            r.Interval(retryCount: 3, interval: TimeSpan.FromSeconds(10));
        });
    }
}