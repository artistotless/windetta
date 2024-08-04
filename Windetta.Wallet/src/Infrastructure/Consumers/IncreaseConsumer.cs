using MassTransit;
using Windetta.Contracts.Events;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Application.Services;
using Windetta.Wallet.Exceptions;
using Windetta.Wallet.Infrastructure.Consumers;

namespace Windetta.Wallet.Infrastructure.Consumers
{
    public class IncreaseConsumer : IConsumer<IIncreaseBalance>
    {
        private readonly IUserWalletService _walletService;

        public IncreaseConsumer(IUserWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task Consume(ConsumeContext<IIncreaseBalance> context)
        {
            var arg = new IncreaseArgument(context.Message.Data, context.Message.Type);

            await _walletService.IncreaseBalance(arg);
        }
    }
}

public class TopUpConsumerDefinition : ConsumerDefinition<IncreaseConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<IncreaseConsumer> consumerConfigurator, IRegistrationContext context)
    {
        consumerConfigurator.UseScheduledRedelivery(r =>
        {
            r.Ignore<WalletException>();
            r.Intervals(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(4), TimeSpan.FromMinutes(10));
        });

        consumerConfigurator.UseMessageRetry(r =>
        {
            r.Ignore<WalletException>();
            r.Interval(retryCount: 3, interval: TimeSpan.FromSeconds(10));
        });
    }
}
