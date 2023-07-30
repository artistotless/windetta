using Windetta.Wallet.Infrastructure.Models;

namespace Windetta.Wallet.Infrastructure.Services;

public interface ITonService
{
    Task<TonWallet> GenerateWallet();
    Task<long> GetBalance(string address);

    /// <summary>
    /// Transfers specified amount of nanotons from user's balance
    /// </summary>
    /// <param name="from">Source wallet credential</param>
    /// <param name="to">Destination TON address</param>
    /// <param name="nanotonAmount">amount of nanotons</param>
    Task<TransferInfo> TransferTon(TonWalletCredential from, string to, long nanotonAmount);
    Task<long> EstimateFees(TonWalletCredential from, string to, long nanotonAmount);
}
