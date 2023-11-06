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
    public async Task ParseTxns()
    {
        var txns = (await _client.GetTransactions(
            new Address(_dest),
            limit: 100
            //to_lt: 16133341000001
            )).Where(x => x.IsInitiator);
        ;
    }
}
