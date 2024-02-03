using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Wallet.Application.Services;
using Windetta.Wallet.Exceptions;

namespace Windetta.Wallet.Infrastructure.Consumers;

public class UnHoldConsumer : IConsumer<IUnHoldBalances>
{
    private readonly IUserWalletService _walletService;

    public UnHoldConsumer(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task Consume(ConsumeContext<IUnHoldBalances> context)
    {
        var funds = context.Message.Funds;
        var users = context.Message.UsersIds;

        await _walletService.UnHoldBalanceAsync(users, funds);

        await context.Publish<IBalancesUnHeld>(new
        {
            context.Message.CorrelationId
        });
    }
}

public class UnHoldConsumerDefinition : ConsumerDefinition<UnHoldConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<UnHoldConsumer> consumerConfigurator, IRegistrationContext context)
    {
        consumerConfigurator.UseInMemoryOutbox(context);
        consumerConfigurator.UseMessageRetry(r =>
        {
            r.Ignore<WalletException>();
            r.Interval(retryCount: 3, interval: TimeSpan.FromSeconds(5));
        });
    }
}