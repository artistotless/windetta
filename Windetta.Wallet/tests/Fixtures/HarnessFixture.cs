using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Wallet.Application.Consumers;
using Windetta.Wallet.Application.Services;
using Windetta.WalletTests.Mocks;

namespace Windetta.WalletTests;

public class HarnessFixture
{
    public Mock<IUserWalletService> UserWalletServiceMock { get; private set; }
    public ITestHarness Harness { get; private set; }

    public HarnessFixture()
    {
        UserWalletServiceMock = new UserWalletServiceMock().Mock;

        var services = new ServiceCollection()
        .AddScoped(x => UserWalletServiceMock.Object)
        .AddMassTransitTestHarness(cfg =>
        {
            cfg.SetEndpointNameFormatter(new MyEndpointNameFormatter(Svc.Wallet));
            cfg.AddConsumers(typeof(CreateConsumer).Assembly);

        }).BuildServiceProvider(true);

        Harness = services.GetRequiredService<ITestHarness>();
        Harness.Start().GetAwaiter().GetResult();
    }
}
