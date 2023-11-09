using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.TonTxns.Infrastructure.Consumers;

namespace Windetta.TonTxnsTests.ConsumersTests;

public class BatchSendTonsConsumerTests : IUsesHarness
{
    private readonly ITestHarness _harness;

    public BatchSendTonsConsumerTests()
    {
        var services = new ServiceCollection()
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider();

        _harness = services.GetRequiredService<ITestHarness>();
    }

    public Action<IBusRegistrationConfigurator> ConfigureHarness()
    {
        return cfg =>
        {
            cfg.AddConsumer<BatchSendTonsConsumer, TestTransferConsumerDefinition>();
        };
    }

    [Fact]
    public async Task ShouldConsumeManyMessagesAtaTime()
    {
        // arrange
        var exptectedCount = 30;
        await _harness.Start();
        var consumerHarness = _harness.GetConsumerHarness<BatchSendTonsConsumer>();
        var transfers = new Fixture().Build<SendTonsImpl>()
            .With(x => x.Destination, new TonAddress("EQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLoLV"))
            .CreateMany(exptectedCount);

        // act
        await _harness.Bus.PublishBatch<ISendTons>(transfers);

        // assert
        _harness.Published.Select<ISendTons>().Count().ShouldBe(exptectedCount);
        (await consumerHarness.Consumed.Any<Batch<ISendTons>>(
            x => x.Context.Message.Length == exptectedCount)).ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldPublishPackedTransferCommand()
    {
        // arrange
        var batchCount = 30;

        var transfers = new Fixture().Build<SendTonsImpl>()
            .With(x => x.Destination, new TonAddress("EQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLoLV"))
            .CreateMany(batchCount);

        // act
        await _harness.Start();
        await _harness.Bus.PublishBatch<ISendTons>(transfers);

        // assert
        (await _harness.Sent.Any<IPackedSendTons>(x =>
        x.Context.Message.Sends.Length == batchCount)).ShouldBe(true);
    }
}


public class SendTonsImpl : ISendTons
{
    public long Nanotons { get; set; }
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