using Windetta.TonTxns.Application.Models;
using Windetta.TonTxns.Application.Services;

namespace Windetta.TonTxns.Infrastructure.Services;

public class TransactionMapper : ITransactionMapper
{
    public FundsFoundData Map(Transaction txn)
    {
        return new FundsFoundData()
        {
            Amount = txn.Amount,
            UserId = Guid.TryParse(txn.Message, out Guid res) ? res : Guid.Empty,
            Id = txn.Id,
        };
    }
}
