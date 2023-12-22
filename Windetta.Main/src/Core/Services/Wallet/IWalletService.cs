using Windetta.Common.Types;
using Windetta.Contracts;

namespace Windetta.Main.Core.Services.Wallet;

public interface IWalletService : IScopedService
{
    Task<UserBalance> GetBalance(Guid userId, int currencyId);
    Task<bool> IsEqualOrGreater(Guid userId, FundsInfo funds);
}
