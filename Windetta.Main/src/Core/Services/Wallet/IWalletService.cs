using Windetta.Common.Types;
using Windetta.Contracts;
using Windetta.Contracts.Responses;

namespace Windetta.Main.Core.Services.Wallet;

/// <summary>
/// Service communicating with the wallet microservice a black box
/// </summary>
public interface IWalletService : IScopedService
{
    /// <summary>
    /// Gets the user's wallet balance
    /// </summary>
    /// <param name="userId">User ID / Wallet ID</param>
    /// <param name="currencyId">Identifier of the currency whose balance need to know</param>
    /// <returns></returns>
    Task<UserBalanceResponse> GetBalance(Guid userId, int currencyId);

    /// <summary>
    /// Checks whether the user has sufficient funds
    /// </summary>
    /// <param name="userId">User ID / Wallet ID</param>
    /// <param name="funds">Description of currency and its quantity</param>
    /// <returns>True - if user has enough money in your wallet, false - if does not.</returns>
    Task<bool> IsEqualOrGreater(Guid userId, FundsInfo funds);
}
