using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.TonTxns.Application.Models;
using Windetta.TonTxns.Application.Services;
using Windetta.TonTxns.Domain;
using Windetta.TonTxns.Infrastructure.Consumers;
using Windetta.TonTxnsTests.Mocks;

namespace Windetta.TonTxnsTests.ConsumersTests;

public class SendTonsConsumerTests : IUsesHarness
{
    public Action<IBusRegistrationConfigurator> ConfigureHarness()
    {
        return cfg =>
        {
            cfg.AddConsumer<SendTonsConsumer>();
        };
    }

    [Fact]
    public async Task ShouldReturnIfTxnAlreadyExistInAnyState()
    {
        // arrange
        var services = new ServiceCollection()
            .AddScoped(x => new TxnsServiceMock(useExistingTxnCase: true).Mock.Object)
            .AddScoped(x => new Mock<IWithdrawService>().Object)
            .AddScoped(x => new Mock<IWalletCredentialSource>().Object)
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider();

        var harness = services.GetRequiredService<ITestHarness>();
        await harness.Start();

        var command = new PackedSendTonsImpl()
        {
            CorrelationId = Guid.NewGuid(),
            Sends = null!
        };

        var consumer = harness.GetConsumerHarness<SendTonsConsumer>();
        var endpoint = await harness.Bus.GetSendEndpoint(
            MyEndpointNameFormatter.CommandUri<IPackedSendTons>(Svc.TonTxns));

        // act
        await endpoint.Send(command);

        // assert
        (await harness.Sent.Any<IPackedSendTons>()).ShouldBeTrue();
        (await harness.Published.Any<Fault<IPackedSendTons>>(
            x => x.Context.Message.Exceptions.FirstOrDefault()?
            .Message == nameof(TxnsServiceMock.UseExistingTxnCase)))
            .ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldHitAllStages()
    {
        // arrange
        var tonServiceMock = new TonServiceMock().Mock;
        var txnsServiceMock = new TxnsServiceMock().Mock;
        var walletCredentialSourceMock = new WalletCredentialSourceMock().Mock;

        var services = new ServiceCollection()
            .AddScoped(x => tonServiceMock.Object)
            .AddScoped(x => txnsServiceMock.Object)
            .AddScoped(x => walletCredentialSourceMock.Object)
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider();

        var harness = services.GetRequiredService<ITestHarness>();
        await harness.Start();

        var sendsCount = 10;
        var command = new PackedSendTonsImpl()
        {
            CorrelationId = Guid.NewGuid(),
            Sends = new Fixture()
            .Build<SendTonsImpl>()
            .With(x => x.Destination, new TonAddress("EQBWfV4S6FXo-EJzSd8QhE7XsmiwxXSsSSBXSC3x8t2KwuVa"))
            .CreateMany(count: sendsCount)
            .ToArray()
        };

        var consumer = harness.GetConsumerHarness<SendTonsConsumer>();
        var endpoint = await harness.Bus.GetSendEndpoint(
            MyEndpointNameFormatter.CommandUri<IPackedSendTons>(Svc.TonTxns));

        // act
        await endpoint.Send(command);

        // assert
        (await harness.Sent.Any<IPackedSendTons>()).ShouldBeTrue();
        (await consumer.Consumed.Any<IPackedSendTons>()).ShouldBeTrue();
        txnsServiceMock.Verify(x => x.ExistAsync(command.CorrelationId));
        txnsServiceMock.Verify(x => x.AddAsync(It.Is<Withdrawal>(x => x.Id == command.CorrelationId)));
        tonServiceMock.Verify(x => x.ExecuteWithdraw(It.IsAny<TonWalletCredential>(), It.IsAny<IEnumerable<TransferMessage>>()));
        tonServiceMock.Verify(x => x.ExecuteWithdraw(It.IsAny<TonWalletCredential>(), It.IsAny<IEnumerable<TransferMessage>>()));
        harness.Published.Select<ISendTonsCompleted>().Count().ShouldBe(sendsCount);
        txnsServiceMock.Verify(x => x.UpdateAsync(It.Is<Withdrawal>(x =>
        x.Id == command.CorrelationId &&
        x.Status == WithdrawalStatus.Confirmed)));
    }
}

public class PackedSendTonsImpl : IPackedSendTons
{
    public Guid CorrelationId { get; set; }
    public ISendTons[] Sends { get; set; }
}