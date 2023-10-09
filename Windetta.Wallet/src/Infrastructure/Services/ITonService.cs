using Windetta.Common.Types;
using Windetta.Wallet.Infrastructure.Models;

namespace Windetta.Wallet.Infrastructure.Services;

public interface ITonService : IScopedService
{
    Task<TonWallet> GenerateWalletData();
    Task<long> GetBalance(string address);

    /// <summary>
    /// Transfers specified amount of nanotons from user's balance
    /// </summary>
    /// <param name="from">Source wallet credential</param>
    /// <param name="to">Destination TON address</param>
    /// <param name="nanotons">amount of nanotons</param>
    Task<TransferResult> TransferTon(TonWalletCredential from, string to, long nanotons);
    Task<long> EstimateFees(TonWalletCredential from, string to, long nanotons);
}
