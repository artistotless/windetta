using MassTransit;
using Windetta.Contracts.Events;
using Windetta.Contracts.Responses;
using Windetta.Wallet.Application.Services;
using Windetta.Wallet.Exceptions;

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
        try
        {
            var balance = await _walletService.GetBalance
                (context.Message.UserId, context.Message.CurrencyId);

            await context.RespondAsync(new UserBalanceResponse(balance.Amount, balance.HeldAmount));
        }
        catch (WalletException e)
        {
            await context.RespondAsync(new UserBalanceResponse(0, 0)
            {
                Error = e.Message
            });
        }
    }
}