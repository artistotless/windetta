using MassTransit;
using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Consumers
{
    public class CreationConsumer : IConsumer<ICreateUserWallet>
    {
        private readonly IUserWalletService _walletService;
        private readonly IBus _bus;

        public CreationConsumer(IUserWalletService walletService, IBus bus)
        {
            _walletService = walletService;
            _bus = bus;
        }

        public async Task Consume(ConsumeContext<ICreateUserWallet> context)
        {
            //var wallet = await _walletService.CreateWalletAsync(context.Message.UserId);

            //await _bus.Publish<IUserWalletCreated>(new
            //{
            //    UserId = context.Message.UserId,
            //    Address = new TonAddress(wallet.Address),
            //    TimeStamp = DateTime.UtcNow,
            //});

            await Task.CompletedTask;
        }
    }
}
