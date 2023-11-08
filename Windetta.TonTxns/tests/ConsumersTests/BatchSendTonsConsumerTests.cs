using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.TonTxns.Infrastructure.Consumers;

namespace Windetta.TonTxnsTests.ConsumersTests;

public class BatchSendTonsConsumerTests : IClassFixture<HarnessFixture>
{
    private readonly HarnessFixture _fixture;

    public BatchSendTonsConsumerTests()
    {
        _fixture = HarnessFixture.Create();
    }

    [Fact]
    public async Task ShouldConsumeManyMessagesAtaTime()
    {
        // arrange
        var exptectedCount = 30;
        var consumerHarness = _fixture.Harness.GetConsumerHarness<BatchSendTonsConsumer>();
        var transfers = new Fixture().Build<SendTonsImpl>()
            .With(x => x.Destination, new TonAddress("EQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLoLV"))
            .CreateMany(exptectedCount);

        // act
        await _fixture.Harness.Bus.PublishBatch<ISendTons>(transfers);

        // assert
        _fixture.Harness.Published.Select<ISendTons>().Count().ShouldBe(exptectedCount);
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
        await _fixture.Harness.Bus.PublishBatch<ISendTons>(transfers);

        // assert
        (await _fixture.Harness.Sent.Any<IPackedSendTons>(x =>
        x.Context.Message.Sends.Length == batchCount)).ShouldBe(true);
    }
}


public class SendTonsImpl : ISendTons
{
    public long Nanotons { get; set; }
    public TonAddress Destination { get; set; }
    public Guid CorrelationId { get; set; }
}