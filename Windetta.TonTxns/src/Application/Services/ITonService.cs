using Windetta.Common.Types;
using Windetta.TonTxns.Infrastructure.Models;

namespace Windetta.TonTxns.Application.Services;

public interface ITonService : IScopedService
{
    Task<long> GetBalance(TonAddress address);

    /// <summary>
    /// Transfers specified amount of nanotons from user's balance
    /// </summary>
    /// <param name="from">Source wallet credential</param>
    /// <param name="to">Destination TON address</param>
    /// <param name="nanotons">amount of nanotons</param>
    Task SendTons(TonWalletCredential from, IEnumerable<TransferMessage> messages);
}
