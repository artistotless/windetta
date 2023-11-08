using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Operations.Sagas;

namespace Windetta.TonTxnsTests;

public class HarnessFixture
{
    public ITestHarness Harness { get; private set; }

    public HarnessFixture()
    {
        var assembly = typeof(TonWithdrawFlow).Assembly;
        var provider = new ServiceCollection()
        .AddMassTransitTestHarness(cfg =>
        {
            cfg.AddPublishMessageScheduler();

            cfg.UsingInMemory((context, cfg) =>
            {
                cfg.UsePublishMessageScheduler();
                cfg.ConfigureEndpoints(context);
            });

            cfg.SetEndpointNameFormatter(new MyEndpointNameFormatter(Svc.Operations));
            cfg.AddSagaStateMachines(assembly);

        }).BuildServiceProvider(true);

        Harness = provider.GetRequiredService<ITestHarness>();
        Harness.Start().GetAwaiter().GetResult();
    }
}