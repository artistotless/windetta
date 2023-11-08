using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.TonTxns.Application.Services;
using Windetta.TonTxns.Infrastructure.Consumers;
using Windetta.TonTxnsTests.Mocks;

namespace Windetta.TonTxnsTests;

public class HarnessFixture
{
    public ITestHarness Harness { get; private set; }

    public Mock<ITonService> TonServiceMock { get; private set; }
    public Mock<ITransactionsService> TxnsServiceMock { get; private set; }
    public Mock<IWalletCredentialSource> WalletCredentialSourceMock { get; private set; }

    public static HarnessFixture Create()
    {
        var fixure = new HarnessFixture();

        fixure.TonServiceMock = new TonServiceMock().Mock;
        fixure.TxnsServiceMock = new TxnsServiceMock().Mock;
        fixure.WalletCredentialSourceMock = new WalletCredentialSourceMock().Mock;

        var provider = new ServiceCollection()
        .AddScoped(x => fixure.TonServiceMock.Object)
        .AddScoped(x => fixure.TxnsServiceMock.Object)
        .AddScoped(x => fixure.WalletCredentialSourceMock.Object)
        .AddMassTransitTestHarness(cfg =>
        {
            cfg.SetEndpointNameFormatter(new MyEndpointNameFormatter(Svc.TonTxns));
            cfg.AddConsumer<BatchSendTonsConsumer, TestTransferConsumerDefinition>();
            cfg.AddConsumer<SendTonsConsumer>();
        }).BuildServiceProvider(true);

        fixure.Harness = provider.GetRequiredService<ITestHarness>();
        fixure.Harness.Start().GetAwaiter().GetResult();

        return fixure;
    }
}

public class TestTransferConsumerDefinition : ConsumerDefinition<BatchSendTonsConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<BatchSendTonsConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.PrefetchCount = 100;
        consumerConfigurator.Options<BatchOptions>(options => options
        .SetMessageLimit(100)
        .SetConcurrencyLimit(4)
        .SetTimeLimit(TimeSpan.FromMilliseconds(1)));
    }
}
