using Windetta.Common.Types;
using Windetta.TonTxns.Application.Models;

namespace Windetta.TonTxns.Application.Services;

public interface ITransactionMapper : ISingletonService
{
    FundsFoundData Map(Transaction txn);
}