using Windetta.Common.Types;

namespace Windetta.Main.Core.Services.Wallet;

public interface IWalletService : IScopedService
{
    Task<UserBalance> GetBalance(Guid userId, int currencyId);
    Task HoldBalance(Guid userId, FundsInfo funds);
    Task UnHoldBalance(Guid userId, int currencyId);
    Task<bool> IsEqualOrGreater(Guid userId, FundsInfo funds);
}
