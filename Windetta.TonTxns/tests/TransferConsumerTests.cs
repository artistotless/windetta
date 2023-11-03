using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.TonTxns.Application.Consumers;
using Windetta.TonTxns.Infrastructure.Models;
using Windetta.TonTxns.Infrastructure.Services;

namespace Windetta.TonTxnsTests;

public class TransferConsumerTests : IClassFixture<HarnessFixture>
{
    private readonly ITestHarness _harness;
    private readonly Mock<ITonService> _tonServiceMock;

    public TransferConsumerTests(HarnessFixture harnessFixture)
    {
        _harness = harnessFixture.Harness;
        _tonServiceMock = harnessFixture.TonServiceMock;
    }

    [Fact]
    public async Task ShouldConsumeManyMessagesAtaTime()
    {
        // arrange
        var exptectedCount = 30;
        var consumerHarness = _harness.GetConsumerHarness<BatchSendTonsConsumer>();
        var transfers = new Fixture().Build<TransferTonImpl>()
            .With(x => x.Destination, new TonAddress("EQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLoLV"))
            .CreateMany(exptectedCount);

        // act
        await _harness.Bus.PublishBatch<ISendTons>(transfers);

        // assert
        (await consumerHarness.Consumed.Any<Batch<ISendTons>>(
            x => x.Context.Message.Length == exptectedCount)).ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Consume_And_TransferTons()
    {
        // arrange
        var batchCount = 30;

        var transfers = new Fixture().Build<TransferTonImpl>()
            .With(x => x.Destination, new TonAddress("EQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLoLV"))
            .CreateMany(batchCount);

        // act
        await _harness.Bus.PublishBatch<ISendTons>(transfers);
        await Task.Delay(200);

        // assert
        _tonServiceMock.Verify(x => x.SendTons(
                It.IsAny<TonWalletCredential>(),
                It.Is<IEnumerable<TransferMessage>>(x => x.Count() == batchCount)), Times.Once);
    }
}


public class TransferTonImpl : ISendTons
{
    public long Nanotons { get; set; }
    public TonAddress Destination { get; set; }
    public Guid CorrelationId { get; set; }
}