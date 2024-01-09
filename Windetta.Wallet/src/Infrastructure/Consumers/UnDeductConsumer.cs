using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Wallet.Application.Services;
using Windetta.Wallet.Exceptions;

namespace Windetta.Wallet.Infrastructure.Consumers;

public class UnDeductConsumer : IConsumer<IUnDeductBalance>
{
    private readonly IUserWalletService _walletService;

    public UnDeductConsumer(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task Consume(ConsumeContext<IUnDeductBalance> context)
    {
        await _walletService.CancelDeductAsync(context.Message.CorrelationId);
    }
}

public class UnDeductConsumerDefinition : ConsumerDefinition<UnDeductConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<UnDeductConsumer> consumerConfigurator, IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(r =>
        {
            r.Ignore<WalletException>();
            r.Interval(retryCount: 3, interval: TimeSpan.FromSeconds(5));
        });
    }
}