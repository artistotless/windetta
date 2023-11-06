using MassTransit.Testing;
using Shouldly;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Wallet.Application.Consumers;

namespace Windetta.WalletTests.ConsumersTests;

public class ConsumersTests : IClassFixture<HarnessFixture>
{
    private readonly ITestHarness _harness;

    public ConsumersTests(HarnessFixture harnessFixture)
    {
        _harness = harnessFixture.Harness;
    }

    [Fact]
    public async Task CreateConsumer_ShouldRespondsToEvent()
    {
        // arrange
        var userId = Guid.NewGuid();

        // act
        await _harness.Bus.Publish<ICreateUserWallet>(new
        {
            UserId = userId
        });
        // assert
        (await _harness.Published.Any<ICreateUserWallet>()).ShouldBeTrue();
        var consumerHarness = _harness.GetConsumerHarness<CreateConsumer>();
        (await consumerHarness.Consumed.Any<ICreateUserWallet>(
        x => x.Context.Message.UserId == userId)).ShouldBeTrue();
    }

    private const string address = "EQCNkSLURL98zKoKQeEoMSCb7uMO5JFWF5CEaJ-f1baspjA2";

    [Fact]
    public async Task DeductConsumer_ShouldRespondsToEvent()
    {
        // arrange
        var command = new DeductBalanceImpl()
        {
            CorrelationId = Guid.NewGuid(),
            Amount = 100,
            UserId = Guid.NewGuid()
        };

        var consumerHarness = _harness.GetConsumerHarness<DeductConsumer>();
        var endpoint = await _harness.Bus.GetSendEndpoint(
            MyEndpointNameFormatter.CommandUri<IDeductBalance>(Svc.Wallet));

        // act
        await endpoint.Send(command);

        // assert
        (await _harness.Sent.Any<IDeductBalance>()).ShouldBeTrue();
        (await consumerHarness.Consumed.Any<IDeductBalance>(
        x => x.Context.Message.CorrelationId == command.CorrelationId))
        .ShouldBeTrue();
    }
}

public class DeductBalanceImpl : IDeductBalance
{
    public Guid UserId { get; set; }
    public long Amount { get; set; }
    public Guid CorrelationId { get; set; }
}