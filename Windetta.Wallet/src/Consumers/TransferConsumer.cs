using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Consumers;

public class TransferConsumer : IConsumer<ITransferTon>
{
    private readonly IUserWalletService _walletService;

    public TransferConsumer(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task Consume(ConsumeContext<ITransferTon> context)
    {
        var userId = context.Message.InitiatorUserId;
        var nanotons = context.Message.Nanotons;
        var destination = context.Message.DestinationUserId;

        await _walletService.TransferAsync(userId, nanotons, destination);
    }
}
