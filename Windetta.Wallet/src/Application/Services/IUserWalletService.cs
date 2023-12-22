using Windetta.Common.Types;
using Windetta.Contracts;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application.Services;

public interface IUserWalletService : IScopedService
{
    Task<UserBalance> GetBalance(Guid userId, int currencyId);
    Task CreateWalletAsync(Guid userId, IEnumerable<UserBalance>? initial = null);
    Task HoldBalanceAsync(Guid userId, FundsInfo funds);
    Task HoldBalanceAsync(IEnumerable<Guid> userIds, FundsInfo funds);
    Task UnHoldBalanceAsync(Guid userId, int currencyId);
    Task TransferAsync(TransferArgument arg);
    Task DeductAsync(DeductArgument arg);
    Task CancelDeductAsync(Guid operationId);
    Task TopUpBalance(TopUpArgument arg);
}