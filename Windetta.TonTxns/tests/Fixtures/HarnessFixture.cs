using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.TonTxns.Consumers;
using Windetta.TonTxnsTests.Mocks;

namespace Windetta.TonTxnsTests;

public class HarnessFixture
{
    public ITestHarness Harness { get; private set; }

    public HarnessFixture()
    {
        var services = new ServiceCollection()
        .AddScoped(x => new TonServiceMock().Mock.Object)
        .AddScoped(x => new WalletCredentialSourceMock().Mock.Object)
        .AddMassTransitTestHarness(cfg =>
        {
            cfg.SetEndpointNameFormatter(new MyEndpointNameFormatter(Svc.TonTxns));
            cfg.AddConsumers(typeof(TransferConsumer).Assembly);

        }).BuildServiceProvider(true);

        Harness = services.GetRequiredService<ITestHarness>();
        Harness.Start().GetAwaiter().GetResult();
    }
}
