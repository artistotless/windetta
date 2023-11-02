using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Consumers;

public class UnDeductConsumer :  IConsumer<IUnDeductBalance>
{
    private readonly IUserWalletService _walletService;

    public UnDeductConsumer(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task Consume(ConsumeContext<IUnDeductBalance> context)
    {
        var userId = context.Message.UserId;
        var value = context.Message.Amount;

        await _walletService.DeductAsync(new(userId, value)
        {
            OperationId = context.Message.CorrelationId
        });
    }
}
