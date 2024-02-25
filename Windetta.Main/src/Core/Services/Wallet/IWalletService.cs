using Windetta.Common.Types;
using Windetta.Contracts;
using Windetta.Contracts.Responses;

namespace Windetta.Main.Core.Services.Wallet;

public interface IWalletService : IScopedService
{
    Task<UserBalanceResponse> GetBalance(Guid userId, int currencyId);
    Task<bool> IsEqualOrGreater(Guid userId, FundsInfo funds);
}
