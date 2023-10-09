using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Consumers
{
    public class CreationConsumer : IConsumer<ICreateUserWallet>
    {
        private readonly IUserWalletService _walletService;

        public CreationConsumer(IUserWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task Consume(ConsumeContext<ICreateUserWallet> context)
        {
            await _walletService.CreateWalletAsync(context.Message.UserId);
        }
    }
}
