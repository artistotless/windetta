using Microsoft.Extensions.Options;
using System.Numerics;
using TonSdk.Client;
using TonSdk.Core;
using Windetta.TonTxns.Application.Models;
using Windetta.TonTxns.Application.Services;
using Windetta.TonTxns.Infrastructure.Extensions;

namespace Windetta.TonTxns.Infrastructure.Services;

public class TransactionLoader : ITransactionsLoader
{
    private readonly TonClient _client;
    private readonly Address _depositAddress;

    public TransactionLoader(
        IOptions<TonClientParameters> parameters,
        IOptions<DepositAddressSource> addressSource)
    {
        _client = new TonClient(parameters.Value);

        if (string.IsNullOrEmpty(addressSource.Value?.Address))
            throw new ArgumentNullException(nameof(addressSource));

        _depositAddress = new Address(addressSource.Value.Address);
    }

    public async Task<IEnumerable<Transaction>?> LoadAsync(Guid lastId)
    {
        var txns = (await _client.GetTransactions(
            _depositAddress, limit: 100, to_lt: ConvertToLt(lastId)))
            .OrderBy(x => x.Utime)
            .Where(x => x.IsInitiator() == false);

        return txns.Select(MapToTransaction)
            .Where(x => x.Amount > 0);
    }

    private ulong? ConvertToLt(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        var span = new Span<byte>(id.ToByteArray());
        var lower = span.Slice(0, 8);
        ulong lt = BitConverter.ToUInt64(lower);

        return lt;
    }

    private Transaction MapToTransaction(TransactionsInformationResult txnInfo)
    {
        Guid BuildGuid(ulong lt, ulong utime)
        {
            UInt128 combined = utime;
            combined <<= 64;
            combined += lt;

            var span = new Span<byte>(new byte[16]);

            new Span<byte>(((BigInteger)combined)
                .ToByteArray(isUnsigned: true, isBigEndian: false))
                .CopyTo(span);

            return new Guid(span);
        }

        var amount = ulong.TryParse(txnInfo.InMsg.Value.ToNano(), out ulong res) ? res : 0;

        return new Transaction()
        {
            Amount = amount,
            Message = txnInfo.InMsg.Message,
            Id = BuildGuid(txnInfo.TransactionId.Lt, (ulong)txnInfo.Utime),
        };
    }
}
