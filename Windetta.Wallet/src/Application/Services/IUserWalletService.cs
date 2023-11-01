using Windetta.Common.Types;
using Windetta.Wallet.Application.Dto;

namespace Windetta.Wallet.Application.Services;

public interface IUserWalletService : IScopedService
{
    Task<WalletBalanceDto> GetBalance(Guid userId);
    Task<WalletInfoDto> CreateWalletAsync(Guid userId);
    Task HoldBalanceAsync(Guid userId, long nanotons);
    Task UnHoldBalanceAsync(Guid userId);
    Task TransferAsync(TransferArgument arg);
    Task WithdrawAsync(WithdrawArgument arg);
    Task TopUpBalance(TopUpArgument arg);
}