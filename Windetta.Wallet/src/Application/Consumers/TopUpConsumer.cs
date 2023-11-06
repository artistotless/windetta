using MassTransit;
using Windetta.Contracts.Events;
using Windetta.Wallet.Application.Consumers;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Application.Consumers
{
    public class TopUpConsumer : IConsumer<IFundsAdded>
    {
        private readonly IUserWalletService _walletService;

        public TopUpConsumer(IUserWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task Consume(ConsumeContext<IFundsAdded> context)
        {
            var userId = context.Message.UserId;
            var nanotons = context.Message.Amount;

            await _walletService.TopUpBalance(new TopUpArgument(userId, nanotons)
            {
                OperationId = context.Message.CorrelationId,
            });
        }
    }
}

public class TopUpConsumerDefinition : ConsumerDefinition<TopUpConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<TopUpConsumer> consumerConfigurator, IRegistrationContext context)
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
