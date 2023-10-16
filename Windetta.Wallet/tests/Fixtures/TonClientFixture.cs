using TonSdk.Client;

namespace Windetta.WalletTests.Fixtures;

public class TonClientFixture : IDisposable
{
    public TonClient Client { get; init; }

    public TonClientFixture()
    {
        Client = new TonClient(new TonClientParameters()
        {
            ApiKey = "xxx",
            Endpoint = "http://localhost:8081/jsonRPC",
        });
    }

    public void Dispose()
    {
        Client.Dispose();
    }
}


[CollectionDefinition("TonTestsCollection")]
public class TonClientFixtureCollection : ICollectionFixture<TonClientFixture> { }
