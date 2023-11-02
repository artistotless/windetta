using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Consumers;

public class TransferConsumer : IConsumer<ITransferBalance>
{
    private readonly IUserWalletService _walletService;

    public TransferConsumer(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task Consume(ConsumeContext<ITransferBalance> context)
    {
        var userId = context.Message.InitiatorUserId;
        var nanotons = context.Message.Amount;
        var destination = context.Message.DestinationUserId;

        await _walletService.TransferAsync(new(userId, nanotons, destination)
        {
            OperationId = context.Message.CorrelationId
        });
    }
}
