using Windetta.Common.Types;
using Windetta.TonTxns.Application.Models;

namespace Windetta.TonTxns.Application.Services;

public interface IWithdrawService : IScopedService
{
    /// <summary>
    /// Transfers specified amount of nanotons from user's balance
    /// </summary>
    /// <param name="from">Source wallet credential</param>
    /// <param name="to">Destination TON address</param>
    /// <param name="nanotons">amount of nanotons</param>
    Task ExecuteWithdraw(WalletCredential from, IEnumerable<TransferMessage> messages);
}
