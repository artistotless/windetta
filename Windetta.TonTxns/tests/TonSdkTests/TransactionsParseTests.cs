using System.Numerics;
using TonSdk.Client;
using TonSdk.Core;
using Windetta.TonTxnsTests.Fixtures;

namespace Windetta.TonTxnsTests.TonSdkTests;

[Collection("TonTestsCollection")]
public class TransactionsParseTests
{
    private readonly TonClient _client;
    private const uint sub_wallet_id = 698983191;
    private const string _dest = "kQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLjlf";

    public TransactionsParseTests(TonClientFixture tonClientFixture)
    {
        _client = tonClientFixture.Client;
    }

    [Fact]
    public void ParseTxns()
    {
        ulong utime = ulong.MaxValue;
        ulong lt = 989959884457958;

        UInt128 combined = utime;
        combined <<= 64;
        combined += lt;

        var span = new Span<byte>(new byte[16]);

        new Span<byte>(((BigInteger)combined)
            .ToByteArray(isUnsigned: true))
            .CopyTo(span);

        var guid = new Guid(span);


        Span<ulong> part1 = new Span<ulong>();


        var txns = (_client.GetTransactions(
            new Address(_dest),
            limit: 100
            //to_lt: 16133341000001
            ).Result).Where(x => x.IsInitiator == false);
        ;
    }
}
