using TonSdk.Client;

namespace Windetta.TonTxnsTests.Fixtures;

public class TonClientFixture : IDisposable
{
    public TonClient Client { get; init; }

    public TonClientFixture()
    {
        Client = new TonClient(new TonClientParameters()
        {
            //ApiKey = "xxx",
            //Endpoint = "http://localhost:8081/jsonRPC",
            ApiKey = "e64ebe66dad5ee14c9c31db58fbaa8e51934a93c2bccdaae4aad8cd7e9d3c146",
            Endpoint = "https://testnet.toncenter.com/api/v2/jsonRPC",
        });
    }

    public void Dispose()
    {
        Client.Dispose();
    }
}


[CollectionDefinition("TonTestsCollection")]
public class TonClientFixtureCollection : ICollectionFixture<TonClientFixture> { }
