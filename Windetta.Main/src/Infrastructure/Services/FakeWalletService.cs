using Windetta.Contracts;
using Windetta.Contracts.Responses;
using Windetta.Main.Core.Services.Wallet;

namespace Windetta.Main.Infrastructure.Services;

public class FakeWalletService : IWalletService
{
    public Task<UserBalanceResponse> GetBalance(Guid userId, int currencyId)
    {
        return Task.FromResult(new UserBalanceResponse(10000, 0));
    }

    public Task<bool> IsEqualOrGreater(Guid userId, FundsInfo funds)
    {
        return Task.FromResult(true);
    }
}