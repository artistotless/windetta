using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Consumers;

public class DeductConsumer : IConsumer<IDeductBalance>
{
    private readonly IUserWalletService _walletService;
    private readonly IBus _bus;

    public DeductConsumer(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task Consume(ConsumeContext<IDeductBalance> context)
    {
        var userId = context.Message.UserId;
        var value = context.Message.Amount;

        await _walletService.DeductAsync(new(userId, value)
        {
            OperationId = context.Message.CorrelationId
        });
    }
}
