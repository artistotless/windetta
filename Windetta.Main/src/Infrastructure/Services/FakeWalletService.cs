using Windetta.Main.Core.Services;

namespace Windetta.Main.Infrastructure.Services;

public class FakeWalletService : IWalletService
{
    public Task<UserBalance> GetBalance(Guid userId, int currencyId)
    {
        return Task.FromResult(new UserBalance(10_000_000_000, 0));
    }

    public Task HoldBalance(Guid userId, int currencyId, ulong amount)
    {
        return Task.CompletedTask;
    }

    public Task<bool> IsEqualOrGreater(Guid userId, int currencyId, ulong amount)
    {
        return Task.FromResult(true);
    }

    public Task UnHoldBalance(Guid userId, int currencyId)
    {
        return Task.CompletedTask;
    }
}