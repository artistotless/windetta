using AutoFixture;
using MassTransit;
using MassTransit.Testing;
using Shouldly;
using Windetta.Common.Types;
using Windetta.Contracts.Commands;

namespace Windetta.TonTxnsTests;

public class TransferConsumerTests : IClassFixture<HarnessFixture>
{
    private readonly ITestHarness _harness;

    public TransferConsumerTests(HarnessFixture harnessFixture)
    {
        _harness = harnessFixture.Harness;
    }

    [Fact]
    public async Task ShouldRecievesFewMessagesAtaTime()
    {
        // arrange
        var exptectedCount = 30;
        var transfers = new Fixture().Build<TransferTonImpl>()
            .With(x => x.Destination, new TonAddress("EQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLoLV"))
            .CreateMany(exptectedCount);

        // act
        await _harness.Bus.PublishBatch<ITransferTon>(transfers);
        _harness.Published.Select(x => x.MessageType == typeof(ITransferTon)).Count().ShouldBe(exptectedCount);
    }
}

public class TransferTonImpl : ITransferTon
{
    public long Nanotons { get; set; }
    public TonAddress Destination { get; set; }
    public Guid CorrelationId { get; set; }
}