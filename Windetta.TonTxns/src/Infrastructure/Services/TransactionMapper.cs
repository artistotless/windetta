using System.Numerics;
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
            UserId = Guid.Parse(txn.Message),
            Id = BuildGuid(txn.Id, txn.TimeStamp)
        };
    }

    private Guid BuildGuid(ulong id, ulong timeStamp)
    {
        UInt128 combined = timeStamp;
        combined <<= 64;
        combined += id;

        var span = new Span<byte>(new byte[16]);

        new Span<byte>(((BigInteger)combined)
            .ToByteArray(isUnsigned: true))
            .CopyTo(span);

        return new Guid(span);
    }
}
