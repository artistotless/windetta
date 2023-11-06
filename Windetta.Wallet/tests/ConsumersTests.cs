using MassTransit;
using MassTransit.Testing;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Wallet.Application.Consumers;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Application.Services;

namespace Windetta.WalletTests.ConsumersTests;

public class ConsumersTests : IClassFixture<HarnessFixture>
{
    private readonly ITestHarness _harness;

    private readonly Mock<IUserWalletService> _userWalletServiceMock;

    public ConsumersTests(HarnessFixture harnessFixture)
    {
        _harness = harnessFixture.Harness;
        _userWalletServiceMock = harnessFixture.UserWalletServiceMock;
    }

    [Fact]
    public async Task CreateConsumer_ShouldRespondsToEvent()
    {
        // arrange
        var command = new Fixture().Create<CreateUserWalletImpl>();

        // act, assert
        await ShouldRespondsToCommand<ICreateUserWallet, CreateConsumer>(command);

        _userWalletServiceMock.Verify(
            x => x.CreateWalletAsync(It.Is<Guid>(x => x == command.UserId)));
    }

    [Fact]
    public async Task DeductConsumer_ShouldRespondsToEvent()
    {
        // arrange
        var command = new Fixture().Create<BalanceOperationImpl>();

        // act, assert
        await ShouldRespondsToCommand<IDeductBalance, DeductConsumer>(command);

        _userWalletServiceMock.Verify(
            x => x.DeductAsync(
                It.Is<DeductArgument>(x => x.OperationId == command.CorrelationId)));
    }

    [Fact]
    public async Task TopUpConsumer_ShouldRespondsToEvent()
    {
        // arrange
        var @event = new Fixture().Create<BalanceOperationImpl>();

        // act, assert
        await ShouldRespondsToEvent<IFundsAdded, TopUpConsumer>(@event);

        _userWalletServiceMock.Verify(
            x => x.TopUpBalance(
                It.Is<TopUpArgument>(x => x.OperationId == @event.CorrelationId)));
    }

    [Fact]
    public async Task TransferConsumer_ShouldRespondsToEvent()
    {
        // arrange
        var command = new Fixture().Create<TransferBalanceImpl>();

        // act, assert
        await ShouldRespondsToCommand<ITransferBalance, TransferConsumer>(command);

        _userWalletServiceMock.Verify(
            x => x.TransferAsync(
                It.Is<TransferArgument>(x => x.OperationId == command.CorrelationId)));
    }

    [Fact]
    public async Task UnDeductConsumer_ShouldRespondsToEvent()
    {
        // arrange
        var command = new Fixture().Create<BalanceOperationImpl>();

        // act, assert
        await ShouldRespondsToCommand<IUnDeductBalance, UnDeductConsumer>(command);

        _userWalletServiceMock.Verify(
            x => x.CancelDeductAsync(
                It.Is<Guid>(x => x == command.CorrelationId)));
    }

    private async Task ShouldRespondsToCommand<TCommand, TConsumer>(TCommand command)
        where TConsumer : class, IConsumer<TCommand>
        where TCommand : class, CorrelatedBy<Guid>
    {
        // arrange
        var consumerHarness = _harness.GetConsumerHarness<TConsumer>();

        var endpoint = await _harness.Bus.GetSendEndpoint(
            MyEndpointNameFormatter.CommandUri<TCommand>(Svc.Wallet));

        // act
        await endpoint.Send(command);

        // assert
        (await _harness.Sent.Any<TCommand>()).ShouldBeTrue();
        (await consumerHarness.Consumed.Any<TCommand>(
        x => x.Context.Message.CorrelationId == command.CorrelationId))
            .ShouldBeTrue();
    }

    private async Task ShouldRespondsToEvent<TEvent, TConsumer>(TEvent @event)
        where TConsumer : class, IConsumer<TEvent>
        where TEvent : class, CorrelatedBy<Guid>
    {
        // arrange
        var consumerHarness = _harness.GetConsumerHarness<TConsumer>();

        // act
        await _harness.Bus.Publish<TEvent>(@event);

        // assert
        (await _harness.Published.Any<TEvent>()).ShouldBeTrue();
        (await consumerHarness.Consumed.Any<TEvent>(
        x => x.Context.Message.CorrelationId == @event.CorrelationId))
            .ShouldBeTrue();
    }
}

public class CreateUserWalletImpl : ICreateUserWallet
{
    public Guid UserId { get; set; }

    public Guid CorrelationId { get; set; }
}

public class BalanceOperationImpl : IDeductBalance, IUnDeductBalance, IFundsAdded
{
    public Guid UserId { get; set; }
    public long Amount { get; set; }
    public Guid CorrelationId { get; set; }
}

public class TransferBalanceImpl : ITransferBalance
{
    public Guid InitiatorUserId { get; set; }
    public Guid DestinationUserId { get; set; }
    public long Amount { get; set; }
    public Guid CorrelationId { get; set; }
}