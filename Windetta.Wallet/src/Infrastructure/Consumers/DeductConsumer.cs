using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Wallet.Application.Services;
using Windetta.Wallet.Exceptions;

namespace Windetta.Wallet.Infrastructure.Consumers;

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
        var funds = context.Message.Funds;

        await _walletService.DeductAsync(new(userId, funds)
        {
            Type = context.Message.Type,
            OperationId = context.Message.CorrelationId
        });

        await context.Publish<IBalanceDeducted>(new
        {
            context.Message.CorrelationId
        });
    }
}

public class DeductConsumerDefinition : ConsumerDefinition<DeductConsumer>
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