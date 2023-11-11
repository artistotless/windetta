using Windetta.Common.Types;
using Windetta.TonTxns.Application.Models;

namespace Windetta.TonTxns.Application.Services;

public interface ITransactionsLoader : IScopedService
{
    Task<IEnumerable<Transaction>?> LoadAsync(Guid lastId);
}