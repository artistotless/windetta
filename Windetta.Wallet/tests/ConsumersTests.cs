using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Application.Services;
using Windetta.Wallet.Infrastructure.Consumers;
using Windetta.WalletTests.Mocks;

namespace Windetta.WalletTests.ConsumersTests;

public class ConsumersTests : IUseHarness
{
    private readonly ITestHarness _harness;

    private Mock<IUserWalletService> _walletSvcMock;

    public ConsumersTests()
    {
        _walletSvcMock = new UserWalletServiceMock().Mock;

        var services = new ServiceCollection()
            .AddSingleton(x => _walletSvcMock.Object)
            .ConfigureTestMassTransit(Svc.Wallet, this)
            .BuildServiceProvider();

        _harness = services.GetRequiredService<ITestHarness>();
    }

    public Action<IBusRegistrationConfigurator> ConfigureHarness()
    {
        return cfg =>
        {
            cfg.AddConsumer<CreateConsumer>();
            cfg.AddConsumer<DeductConsumer>();
            cfg.AddConsumer<UnDeductConsumer>();
            cfg.AddConsumer<IncreaseConsumer>();
            cfg.AddConsumer<TransferConsumer>();
        };
    }

    [Fact]
    public async Task CreateConsumer_ShouldRespondsToEvent()
    {
        // arrange
        var @event = new Fixture().Create<UserCreatedImpl>();

        // act, assert
        await ShouldRespondsToEvent<IUserCreated, CreateConsumer>(@event);

        _walletSvcMock.Verify(
            x => x.CreateWalletAsync(It.Is<Guid>(x => x == @event.Id), null));
    }

    [Fact]
    public async Task DeductConsumer_ShouldRespondsToEvent()
    {
        // arrange
        var command = new Fixture().Create<DeductBalanceImpl>();

        command.Type = NegativeBalanceOperationType.Withdrawal;

        // act, assert
        await ShouldRespondsToCommand<IDeductBalance, DeductConsumer>(command);

        _walletSvcMock.Verify(
            x => x.DeductAsync(
                It.Is<DeductArgument>(x => x.OperationId == command.CorrelationId)));
    }

    [Fact]
    public async Task IncreaseConsumer_ShouldRespondsToEvent()
    {
        // arrange
        var command = new Fixture().Create<IncreaseBalanceImpl>();

        // act, assert
        await ShouldRespondsToEvent<IIncreaseBalance, IncreaseConsumer>(command);

        _walletSvcMock.Verify(
            x => x.IncreaseBalance(
                It.Is<IncreaseArgument>(x => x.Data.First().OperationId == command.CorrelationId)));
    }

    [Fact]
    public async Task TransferConsumer_ShouldRespondsToEvent()
    {
        // arrange
        var command = new Fixture().Create<TransferBalanceImpl>();

        // act, assert
        await ShouldRespondsToCommand<ITransferBalance, TransferConsumer>(command);

        _walletSvcMock.Verify(
            x => x.TransferAsync(
                It.Is<TransferArgument>(x => x.OperationId == command.CorrelationId)));
    }

    [Fact]
    public async Task UnDeductConsumer_ShouldRespondsToEvent()
    {
        // arrange
        var command = new Fixture().Create<UnDeductBalanceImpl>();

        // act, assert
        await ShouldRespondsToCommand<IUnDeductBalance, UnDeductConsumer>(command);

        _walletSvcMock.Verify(
            x => x.CancelDeductAsync(
                It.Is<Guid>(x => x == command.CorrelationId)));
    }

    private async Task ShouldRespondsToCommand<TCommand, TConsumer>(TCommand command)
        where TConsumer : class, IConsumer<TCommand>
        where TCommand : class, CorrelatedBy<Guid>
    {
        // arrange
        await _harness.Start();
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
        await _harness.Start();
        var consumerHarness = _harness.GetConsumerHarness<TConsumer>();

        // act
        await _harness.Bus.Publish(@event);

        // assert
        (await _harness.Published.Any<TEvent>()).ShouldBeTrue();
        (await consumerHarness.Consumed.Any<TEvent>(
        x => x.Context.Message.CorrelationId == @event.CorrelationId))
            .ShouldBeTrue();
    }
}

public class UserCreatedImpl : IUserCreated
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public DateTime TimeStamp { get; set; }
    public Guid CorrelationId { get; set; }
}

public class DeductBalanceImpl : IDeductBalance
{
    public Guid UserId { get; set; }
    public FundsInfo Funds { get; set; }
    public Guid CorrelationId { get; set; }
    public NegativeBalanceOperationType Type { get; set; }
}

public class IncreaseBalanceImpl : IIncreaseBalance
{
    public Guid CorrelationId { get; set; }
    public PositiveBalanceOperationType Type { get; set; }
    public IEnumerable<BalanceOperationData> Data { get; set; }
}

public class UnDeductBalanceImpl : IUnDeductBalance
{
    public Guid CorrelationId { get; set; }
}

public class TransferBalanceImpl : ITransferBalance
{
    public Guid InitiatorUserId { get; set; }
    public Guid DestinationUserId { get; set; }
    public FundsInfo Funds { get; set; }
    public Guid CorrelationId { get; set; }
}