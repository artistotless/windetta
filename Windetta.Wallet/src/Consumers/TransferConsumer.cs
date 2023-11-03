using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Consumers;

public class TransferConsumer : IConsumer<ITransferBalance>
{
    private readonly IUserWalletService _walletService;

    public TransferConsumer(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task Consume(ConsumeContext<ITransferBalance> context)
    {
        var userId = context.Message.InitiatorUserId;
        var nanotons = context.Message.Amount;
        var destination = context.Message.DestinationUserId;

        await _walletService.TransferAsync(new(userId, nanotons, destination)
        {
            OperationId = context.Message.CorrelationId
        });
    }
}

public class TransferConsumerDefinition : ConsumerDefinition<TransferConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<TransferConsumer> consumerConfigurator, IRegistrationContext context)
    {
        consumerConfigurator.UseScheduledRedelivery(r =>
        {
            r.Intervals(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(4), TimeSpan.FromMinutes(10));
        });

        consumerConfigurator.UseMessageRetry(r =>
        {
            r.Interval(retryCount: 3, interval: TimeSpan.FromSeconds(10));
        });
    }
}