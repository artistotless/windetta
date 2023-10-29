using MassTransit;
using Windetta.Contracts.Events;
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
            var nanotons = context.Message.Nanotons;
            var transactionId = context.Message.TxnId;

            await _walletService.IncreaseBalance(userId, nanotons, transactionId);
        }
    }
}
