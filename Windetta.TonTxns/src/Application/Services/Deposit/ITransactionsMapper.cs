using Windetta.TonTxns.Application.Models;

namespace Windetta.TonTxns.Application.Services;

public interface ITransactionsMapper
{
    IEnumerable<FundsAddedData> Map(IEnumerable<Transaction> receipts);
}