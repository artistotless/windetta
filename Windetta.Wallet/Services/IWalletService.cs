using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Services;

public interface IWalletService
{
    Task<Balance> GetBalance(Guid userId);
    Task<string> GetDepositAddress(Guid userId);
    Task TransferTon(TonWalletCredential from, string to, int nanotonAmount);
    Task<int> EstimateFees(TonWalletCredential from, string to, int nanotonAmount);
    Task HoldBalance(Guid userId, int nanotonAmount);
    Task UnHoldBalance(Guid userId);
    Task<UserWallet> CreateWallet(Guid userId);
}