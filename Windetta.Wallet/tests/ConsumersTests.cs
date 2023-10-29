using MassTransit.Testing;
using Shouldly;
using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.Wallet.Consumers;
using Windetta.WalletTests.Fixtures;

namespace Windetta.WalletTests.ConsumersTests;

public class ConsumersTests : IClassFixture<HarnessFixture>
{
    private readonly ITestHarness _harness;

    public ConsumersTests(HarnessFixture harnessFixture)
    {
        _harness = harnessFixture.Harness;
    }

    [Fact]
    public async Task CreationConsumerShouldRespondsToEvent()
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
    public async Task WithdrawConsumerShouldRespondsToEvent()
    {
        // arrange
        var command = new
        {
            Destination = TonAddress.Parse(address),
            Nanotons = 100,
            UserId = Guid.NewGuid()
        };

        // act
        await _harness.Bus.Publish<IWithdrawTon>(command);

        // assert
        (await _harness.Published.Any<IWithdrawTon>()).ShouldBeTrue();

        var consumerHarness = _harness.GetConsumerHarness<WithdrawConsumer>();

        (await consumerHarness.Consumed.Any<IWithdrawTon>(
        x => x.Context.Message.UserId == command.UserId &&
        x.Context.Message.Nanotons == command.Nanotons &&
        x.Context.Message.Destination == command.Destination))
        .ShouldBeTrue();
    }
}