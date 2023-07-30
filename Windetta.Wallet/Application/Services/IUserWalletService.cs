using Windetta.Wallet.Application.Dto;

namespace Windetta.Wallet.Application.Services;

public interface IUserWalletService
{
    Task<WalletBalance> GetBalance(Guid userId);
    Task<string> GetDepositAddress(Guid userId);
    Task HoldBalance(Guid userId, int nanotonAmount);
    Task UnHoldBalance(Guid userId);
}