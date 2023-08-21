using Windetta.Common.Types;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Infrastructure.Models;

namespace Windetta.Wallet.Application.Services;

public interface IUserWalletService : IScopedService
{
    Task<WalletBalance> GetBalance(Guid userId);
    Task<string> GetDepositAddress(Guid userId);
    Task HoldBalance(Guid userId, int nanotonAmount);
    Task UnHoldBalance(Guid userId);
    Task CreateWallet(Guid userId, TonWallet wallet);
}