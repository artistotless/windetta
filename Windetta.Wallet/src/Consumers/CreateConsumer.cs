using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Consumers
{
    public class CreateConsumer : IConsumer<ICreateUserWallet>
    {
        private readonly IUserWalletService _walletService;
        private readonly IBus _bus;

        public CreateConsumer(IUserWalletService walletService, IBus bus)
        {
            _walletService = walletService;
            _bus = bus;
        } 

        public async Task Consume(ConsumeContext<ICreateUserWallet> context)
        {
            await _walletService.CreateWalletAsync(context.Message.UserId);

            await _bus.Publish<IUserWalletCreated>(new
            {
                UserId = context.Message.UserId,
                TimeStamp = DateTime.UtcNow,
            });

            await Task.CompletedTask;
        }
    }
}
