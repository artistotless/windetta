using Windetta.TonTxns.Application.Models;

namespace Windetta.TonTxns.Application.Services;

public interface ITransactionMapper
{
    FundsFoundData Map(Transaction txn);
}