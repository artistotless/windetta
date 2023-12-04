using Windetta.Common.Types;

namespace Windetta.Main.Services;

public interface IWalletService : IScopedService
{
    Task<UserBalance> GetBalance(Guid userId, int currencyId);
    Task HoldBalance(Guid userId, int currencyId, ulong amount);
    Task UnHoldBalance(Guid userId, int currencyId);
    Task<bool> IsEqualOrGreater(Guid userId, int currencyId, ulong amount);
}
