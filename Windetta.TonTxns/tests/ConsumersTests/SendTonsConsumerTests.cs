using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.TonTxns.Application.Models;
using Windetta.TonTxns.Domain;
using Windetta.TonTxns.Infrastructure.Consumers;
using Windetta.TonTxnsTests.Mocks;

namespace Windetta.TonTxnsTests.ConsumersTests;

public class SendTonsConsumerTests
{
    private readonly HarnessFixture _fixture;

    public SendTonsConsumerTests()
    {
        _fixture = HarnessFixture.Create();
    }

    [Fact]
    public async Task ShouldReturnIfTxnAlreadyExistInAnyState()
    {
        // arrange
        var command = new PackedSendTonsImpl()
        {
            CorrelationId = TxnsServiceMock.ExistingTxn,
            Sends = null!
        };

        var consumer = _fixture.Harness.GetConsumerHarness<SendTonsConsumer>();
        var endpoint = await _fixture.Harness.Bus.GetSendEndpoint(
            MyEndpointNameFormatter.CommandUri<IPackedSendTons>(Svc.TonTxns));

        // act
        await endpoint.Send(command);

        // assert
        (await _fixture.Harness.Sent.Any<IPackedSendTons>()).ShouldBeTrue();
        (await consumer.Consumed.Any<IPackedSendTons>()).ShouldBeTrue();
        (await _fixture.Harness.Published.Any<Fault<IPackedSendTons>>(
            x => x.Context.Message.Exceptions.FirstOrDefault()?
            .Message == nameof(TxnsServiceMock.ExistingTxn)))
            .ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldHitAllStages()
    {
        // arrange
        var sendsCount = 10;
        var command = new PackedSendTonsImpl()
        {
            CorrelationId = TxnsServiceMock.NonExistingTxn,
            Sends = new Fixture()
            .Build<SendTonsImpl>()
            .With(x => x.Destination, new TonAddress("EQBWfV4S6FXo-EJzSd8QhE7XsmiwxXSsSSBXSC3x8t2KwuVa"))
            .CreateMany(count: sendsCount)
            .ToArray()
        };

        var consumer = _fixture.Harness.GetConsumerHarness<SendTonsConsumer>();
        var endpoint = await _fixture.Harness.Bus.GetSendEndpoint(
            MyEndpointNameFormatter.CommandUri<IPackedSendTons>(Svc.TonTxns));

        // act
        await endpoint.Send(command);

        // assert
        (await _fixture.Harness.Sent.Any<IPackedSendTons>()).ShouldBeTrue();
        (await consumer.Consumed.Any<IPackedSendTons>()).ShouldBeTrue();
        _fixture.TxnsServiceMock.Verify(x => x.ExistAsync(command.CorrelationId));
        _fixture.TxnsServiceMock.Verify(x => x.AddAsync(It.Is<Transaction>(x => x.Id == command.CorrelationId)));
        _fixture.TonServiceMock.Verify(x => x.SendTons(It.IsAny<TonWalletCredential>(), It.IsAny<IEnumerable<TransferMessage>>()));
        _fixture.TonServiceMock.Verify(x => x.SendTons(It.IsAny<TonWalletCredential>(), It.IsAny<IEnumerable<TransferMessage>>()));
        _fixture.Harness.Published.Select<ISendTonsCompleted>().Count().ShouldBe(sendsCount);
        _fixture.TxnsServiceMock.Verify(x => x.UpdateAsync(It.Is<Transaction>(x =>
        x.Id == command.CorrelationId &&
        x.Status == TransactionStatus.Confirmed)));
    }
}

public class PackedSendTonsImpl : IPackedSendTons
{
    public Guid CorrelationId { get; set; }
    public ISendTons[] Sends { get; set; }
}