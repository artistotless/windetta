using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Wallet.Consumers;
using Windetta.WalletTests.Mocks;

namespace Windetta.WalletTests;

public class HarnessFixture
{
    public ITestHarness Harness { get; private set; }

    public HarnessFixture()
    {
        var services = new ServiceCollection()
        .AddScoped(x => new UserWalletServiceMock().Mock.Object)
        .AddMassTransitTestHarness(cfg =>
        {
            cfg.SetEndpointNameFormatter(new MyEndpointNameFormatter(Svc.Wallet));
            cfg.AddConsumers(typeof(CreationConsumer).Assembly);

        }).BuildServiceProvider(true);

        Harness = services.GetRequiredService<ITestHarness>();
        Harness.Start().GetAwaiter().GetResult();
    }
}
