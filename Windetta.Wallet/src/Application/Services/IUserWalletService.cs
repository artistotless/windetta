using Windetta.Common.Types;
using Windetta.Wallet.Application.Dto;

namespace Windetta.Wallet.Application.Services;

public interface IUserWalletService : IScopedService
{
    Task HoldBalanceAsync(Guid userId, long nanotons);
    Task UnHoldBalanceAsync(Guid userId);
    Task<WalletInfoDto> CreateWalletAsync(Guid userId);
    Task TransferAsync(Guid userId, long nanotons, Guid destinationUser);
    Task WithdrawAsync(Guid userId, long nanotons, TonAddress destinationAddress);
    Task IncreaseBalance(Guid userId, long nanotons,string txnId);
    Task<WalletBalanceDto> GetBalance(Guid userId);
}