using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.TonTxns.Application.Consumers;
using Windetta.TonTxns.Application.Services;
using Windetta.TonTxns.Infrastructure.Services;

namespace Windetta.TonTxnsTests;

public class TransferConsumerTests : IClassFixture<HarnessFixture>
{
    private readonly ITestHarness _harness;

    private readonly Mock<ITonService> _tonServiceMock;
    private readonly Mock<ITransactionsService> _txnsServiceMock;

    public TransferConsumerTests(HarnessFixture harnessFixture)
    {
        _harness = harnessFixture.Harness;
        _tonServiceMock = harnessFixture.TonServiceMock;
        _txnsServiceMock = harnessFixture.TxnsServiceMock;
    }

    [Fact]
    public async Task BatchConsumer_ShouldConsumeManyMessagesAtaTime()
    {
        // arrange
        var exptectedCount = 30;
        var consumerHarness = _harness.GetConsumerHarness<BatchSendTonsConsumer>();
        var transfers = new Fixture().Build<SendTonsImpl>()
            .With(x => x.Destination, new TonAddress("EQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLoLV"))
            .CreateMany(exptectedCount);

        // act
        await _harness.Bus.PublishBatch<ISendTons>(transfers);

        // assert
        (await consumerHarness.Consumed.Any<Batch<ISendTons>>(
            x => x.Context.Message.Length == exptectedCount)).ShouldBeTrue();
    }

    [Fact]
    public async Task BatchConsumer_ShouldPublishPackedTransferCommand()
    {
        // arrange
        var batchCount = 30;

        var transfers = new Fixture().Build<SendTonsImpl>()
            .With(x => x.Destination, new TonAddress("EQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLoLV"))
            .CreateMany(batchCount);

        // act
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

public class PackedSendTonsImpl : IPackedSendTons
{
    public Guid CorrelationId { get; set; }
    public ISendTons[] Sends { get; set; }
}

