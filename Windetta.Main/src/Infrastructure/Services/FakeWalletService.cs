using Windetta.Common.Types;
using Windetta.Main.Core.Services.Wallet;

namespace Windetta.Main.Infrastructure.Services;

public class FakeWalletService : IWalletService
{
    public Task<UserBalance> GetBalance(Guid userId, int currencyId)
    {
        return Task.FromResult(new UserBalance(10_000_000_000, 0));
    }

    public Task HoldBalance(Guid userId, FundsInfo funds)
    {
        return Task.CompletedTask;
    }

    public Task<bool> IsEqualOrGreater(Guid userId, FundsInfo funds)
    {
        return Task.FromResult(true);
    }

    public Task UnHoldBalance(Guid userId, int currencyId)
    {
        return Task.CompletedTask;
    }
}