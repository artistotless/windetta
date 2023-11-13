using MassTransit.Events;
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

public class SendTonsConsumerTests : IHarnessConfiguration
{
    private const string _addr = "EQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLoLV";

    public Action<IBusRegistrationConfigurator> ConfigureHarness()
    {
        return cfg =>
        {
            cfg.AddConsumer<SendTonsConsumer>();
            cfg.AddConsumer<BatchSendTonsConsumer, TestTransferConsumerDefinition>();
        };
    }

    [Fact]
    public async Task ShouldReturnIfTxnAlreadyExistInAnyState()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .AddScoped(x => new TxnsServiceMock(useExistingTxnCase: true).Mock.Object)
            .AddScoped(x => new Mock<IWithdrawService>().Object)
            .AddScoped(x => new Mock<IWalletCredentialSource>().Object)
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

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

        await using var provider = new ServiceCollection()
             .AddScoped(x => tonServiceMock.Object)
             .AddScoped(x => txnsServiceMock.Object)
             .AddScoped(x => walletCredentialSourceMock.Object)
             .ConfigureMassTransit(Svc.TonTxns, this)
             .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();

        var sendsCount = 10;
        var command = new PackedSendTonsImpl()
        {
            CorrelationId = Guid.NewGuid(),
            Sends = new Fixture()
            .Build<SendTonsImpl>()
            .With(x => x.Destination, new TonAddress(_addr))
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
        tonServiceMock.Verify(x => x.ExecuteWithdraw(It.IsAny<WalletCredential>(), It.IsAny<IEnumerable<TransferMessage>>()));
        harness.Published.Select<ISendTonsCompleted>().Count().ShouldBe(sendsCount);
        txnsServiceMock.Verify(x => x.UpdateAsync(It.Is<Withdrawal>(x =>
        x.Id == command.CorrelationId &&
        x.Status == WithdrawalStatus.Confirmed)));
    }

    [Fact]
    public async Task ShouldPublishFaultEventsIfSendTonsFailed()
    {
        // arrange
        var tonServiceMock = new TonServiceMock(useFailCase: true).Mock;
        var txnsServiceMock = new TxnsServiceMock().Mock;
        var walletCredentialSourceMock = new WalletCredentialSourceMock().Mock;

        await using var provider = new ServiceCollection()
            .AddScoped(x => tonServiceMock.Object)
            .AddScoped(x => txnsServiceMock.Object)
            .AddScoped(x => walletCredentialSourceMock.Object)
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();

        var sendsCount = 10;
        var command = new PackedSendTonsImpl()
        {
            CorrelationId = Guid.NewGuid(),
            Sends = new Fixture()
            .Build<SendTonsImpl>()
            .With(x => x.Destination, new TonAddress(_addr))
            .CreateMany(count: sendsCount)
            .ToArray()
        };

        var consumer = harness.GetConsumerHarness<SendTonsConsumer>();
        var endpoint = await harness.Bus.GetSendEndpoint(
            MyEndpointNameFormatter.CommandUri<IPackedSendTons>(Svc.TonTxns));

        // act
        await endpoint.Send(command);

        // assert
        (await consumer.Consumed.Any<IPackedSendTons>()).ShouldBeTrue();
        tonServiceMock.Verify(x => x.ExecuteWithdraw(It.IsAny<WalletCredential>(), It.IsAny<IEnumerable<TransferMessage>>()));
        harness.Published.Select<FaultEvent<ISendTons>>().Count().ShouldBe(sendsCount);
    }

    [Fact]
    public async Task ShouldPublishPackedTransferCommand()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        var batchCount = 30;

        var transfers = new Fixture().Build<SendTonsImpl>()
            .With(x => x.Destination, new TonAddress(_addr))
            .CreateMany(batchCount);

        // act
        await harness.Start();
        await harness.Bus.PublishBatch<ISendTons>(transfers);

        // assert
        (await harness.Sent.Any<IPackedSendTons>())
            .ShouldBeTrue();
    }
}

public class PackedSendTonsImpl : IPackedSendTons
{
    public Guid CorrelationId { get; set; }
    public ISendTons[] Sends { get; set; }
}

public class SendTonsImpl : ISendTons
{
    public ulong Nanotons { get; set; }
    public TonAddress Destination { get; set; }
    public Guid CorrelationId { get; set; }
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