using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Consumers;

public class WithdrawConsumer : IConsumer<IWithdrawTon>
{
    private readonly IUserWalletService _walletService;

    public WithdrawConsumer(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task Consume(ConsumeContext<IWithdrawTon> context)
    {
        var userId = context.Message.UserId;
        var nanotons = context.Message.Nanotons;
        var destination = context.Message.Destination;

        await _walletService.TransferAsync(userId, nanotons, destination);
    }
}
