using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.TonTxns.Consumers;
using Windetta.TonTxns.Infrastructure.Services;
using Windetta.TonTxnsTests.Mocks;

namespace Windetta.TonTxnsTests;

public class HarnessFixture
{
    public ITestHarness Harness { get; private set; }

    public Mock<ITonService> TonServiceMock { get; private set; }
    public Mock<IWalletCredentialSource> WalletCredentialSourceMock { get; private set; }

    public HarnessFixture()
    {
        TonServiceMock = new TonServiceMock().Mock;
        WalletCredentialSourceMock = new WalletCredentialSourceMock().Mock;

        var provider = new ServiceCollection()
        .AddSingleton(x => TonServiceMock.Object)
        .AddSingleton(x => WalletCredentialSourceMock.Object)
        .AddMassTransitTestHarness(cfg =>
        {
            cfg.SetEndpointNameFormatter(new MyEndpointNameFormatter(Svc.TonTxns));
            cfg.AddConsumer<TransferConsumer>(typeof(TestTransferConsumerDefinition));

        }).BuildServiceProvider(true);

        Harness = provider.GetRequiredService<ITestHarness>();
        Harness.Start().GetAwaiter().GetResult();
    }
}

public class TestTransferConsumerDefinition : ConsumerDefinition<TransferConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<TransferConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.PrefetchCount = 100;
        consumerConfigurator.Options<BatchOptions>(options => options
        .SetMessageLimit(100)
        .SetConcurrencyLimit(4)
        .SetTimeLimit(TimeSpan.FromMilliseconds(1)));
    }
}
