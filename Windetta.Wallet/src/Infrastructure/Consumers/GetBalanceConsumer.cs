using MassTransit;
using Windetta.Contracts.Events;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Infrastructure.Consumers;

public class GetBalanceConsumer : IConsumer<IBalanceRequested>
{
    private readonly IUserWalletService _walletService;

    public GetBalanceConsumer(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task Consume(ConsumeContext<IBalanceRequested> context)
    {
        var balance = await _walletService.GetBalance
            (context.Message.UserId, context.Message.CurrencyId);

        await context.RespondAsync(balance);
    }
}
