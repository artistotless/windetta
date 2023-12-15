using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Infrastructure.Consumers;

public class HoldConsumer : IConsumer<IHoldBalances>
{
    private readonly IUserWalletService _walletService;

    public HoldConsumer(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task Consume(ConsumeContext<IHoldBalances> context)
    {
        var funds = context.Message.Funds;
        var users = context.Message.UsersIds;

        await _walletService.HoldBalanceAsync(users, funds);

        await context.Publish<IBalancesHeld>(new
        {
            context.Message.CorrelationId
        });
    }
}

public class HoldConsumerDefinition : ConsumerDefinition<HoldConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<HoldConsumer> consumerConfigurator, IRegistrationContext context)
    {
        consumerConfigurator.UseInMemoryOutbox(context);
        consumerConfigurator.UseMessageRetry(r =>
        {
            r.Interval(retryCount: 3, interval: TimeSpan.FromSeconds(5));
        });
    }
}