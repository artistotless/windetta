using Windetta.Common.Types;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application.Services;

public interface IUserWalletService : IScopedService
{
    Task<UserBalance> GetBalance(Guid userId, int currencyId);
    Task CreateWalletAsync(Guid userId, IEnumerable<UserBalance>? initial = null);
    Task HoldBalanceAsync(Guid userId, int currencyId, ulong amount);
    Task UnHoldBalanceAsync(Guid userId, int currencyId);
    Task TransferAsync(TransferArgument arg);
    Task DeductAsync(DeductArgument arg);
    Task CancelDeductAsync(Guid operationId);
    Task TopUpBalance(TopUpArgument arg);
}