using Windetta.Common.Types;
using Windetta.Wallet.Application.Dto;

namespace Windetta.Wallet.Application.Services;

public interface IUserWalletService : IScopedService
{
    Task<WalletInfoDto> GetWalletInfoAsync(Guid userId);
    Task HoldBalanceAsync(Guid userId, int nanotons);
    Task UnHoldBalanceAsync(Guid userId);
    Task CreateWalletAsync(Guid userId);
    Task TransferAsync(Guid userId, long nanotons, TonAddress destinationAddress);
}