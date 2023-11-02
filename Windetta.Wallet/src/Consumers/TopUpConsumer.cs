using MassTransit;
using Windetta.Contracts.Events;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Consumers
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
