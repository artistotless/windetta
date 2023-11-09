using Windetta.TonTxns.Application.Models;

namespace Windetta.TonTxns.Application.Services;

public interface ITransactionsLoader
{
    Task<IEnumerable<Transaction>> LoadAsync(ulong lastLt);
}