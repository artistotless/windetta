using MassTransit.Events;
using MassTransit.QuartzIntegration;
using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Common.Testing;
using Windetta.Contracts;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.TonTxns.Infrastructure.Sagas;
using Windetta.TonTxnsTests.ConsumersTests;
using Xunit.Abstractions;

namespace Windetta.TonTxnsTests.Sagas;

public class TonWithdrawFlowTests : IUseHarness
{
    private readonly XUnitOutWrapper _output;
    private readonly Guid correllationId;

    private const string _addr = "EQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLoLV";
    private const string correllationIdString = "3bac654b-64db-4914-8d8e-b531ae796d5c";

    public TonWithdrawFlowTests(ITestOutputHelper output)
    {
        _output = new XUnitOutWrapper(output);
        correllationId = Guid.Parse(correllationIdString);
    }

    public Action<IBusRegistrationConfigurator> ConfigureHarness()
    {
        return cfg =>
        {
            cfg.AddSagaStateMachine<TonWithdrawFlowStateMachine, TonWithdrawFlow>();
        };
    }

    private TonWithdrawFlow CreateSagaWithState(TonWithdrawFlowState initialState)
    {
        return new TonWithdrawFlow
        {
            UserId = Guid.NewGuid(),
            Destination = new TonAddress(_addr),
            Nanotons = 1_000_000_000uL,
            CurrentState = (int)initialState,
            CorrelationId = correllationId,
        };
    }

    [Fact]
    public async Task WhenWithdrawRequested()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureTestMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();
        var argument = new
        {
            UserId = Guid.NewGuid(),
            Destination = new TonAddress(_addr),
            CorrelationId = Guid.NewGuid(),
            Nanotons = 1_000_000_000uL
        };

        var sagaHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();

        // act
        await harness.Bus.Publish<IWithdrawTonRequested>(argument);

        // assert
        (await harness.Consumed.Any<IWithdrawTonRequested>())
            .ShouldBe(true);
        (await sagaHarness.Consumed.Any<IWithdrawTonRequested>())
            .ShouldBe(true);
        (await sagaHarness.Exists(argument.CorrelationId, s => s.AwaitingDeduction))
            .HasValue.ShouldBeTrue();

        await harness.OutputTimeline(_output, x => x.Now()); ;
    }

    [Fact]
    public async Task WhenBalanceDeducted()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureTestMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (TonWithdrawFlowState.AwaitingDeduction));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();

        // act
        await harness.Bus.Publish<IBalanceDeducted>(new BalanceDeductedImpl()
        {
            CorrelationId = correllationId
        });

        // assert
        await sagaHarness.Consumed.Any<IBalanceDeducted>();
        // check schedule
        (await harness.Sent.Any<ISendTons>())
            .ShouldBeTrue();
        (await sagaHarness.Exists(correllationId, s => s.BalanceDeductedSuccess))
            .HasValue.ShouldBeTrue();
    }

    [Fact]
    public async Task WhenBalanceDeductedFailed()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureTestMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (TonWithdrawFlowState.AwaitingDeduction));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();

        // act
        await harness.Bus.Publish(new FaultEvent<IDeductBalance>()
        {
            Message = new DeductBalanceImpl()
            {
                CorrelationId = correllationId,
            }
        });
        await sagaHarness.Consumed.Any<Fault<IBalanceDeducted>>();

        // assert
        (await sagaHarness.Exists(correllationId, s => s.BalanceDeductFail))
            .HasValue.ShouldBeTrue();
    }

    [Fact]
    public async Task WhenSendTonsCompleted()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureTestMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (TonWithdrawFlowState.BalanceDeductedSuccess));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();

        // act
        await harness.Bus.Publish<ISendTonsCompleted>(new SendTonsCompletedImpl()
        {
            CorrelationId = correllationId
        });

        // assert
        (await sagaHarness.Consumed.Any<ISendTonsCompleted>())
            .ShouldBeTrue();
        (await sagaHarness.Exists(correllationId, s => s.Final))
            .HasValue.ShouldBeTrue();
    }

    [Fact]
    public async Task WhenSendTonsFailed()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureTestMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (TonWithdrawFlowState.BalanceDeductedSuccess));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();

        // act
        await harness.Bus.Publish(new FaultEvent<ISendTons>()
        {
            Message = new SendTonsImpl() { CorrelationId = correllationId }
        });

        // assert
        (await sagaHarness.Consumed.Any<Fault<ISendTons>>())
            .ShouldBeTrue();
        (await harness.Sent.Any<IUnDeductBalance>())
            .ShouldBeTrue();
        (await sagaHarness.Exists(correllationId, s => s.SendTonsFail))
            .HasValue.ShouldBeTrue();
    }

    [Fact]
    public async Task WhenBalanceUnDeductFailed()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureTestMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (TonWithdrawFlowState.SendTonsFail));

        var stateMachineHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();

        // act
        await harness.Bus.Publish(new FaultEvent<IUnDeductBalance>()
        {
            Message = new UnDeductBalanceImpl() { CorrelationId = correllationId },
        });

        // assert
        (await harness.Sent.Any<INotifyUnDeductBalanceFailed>()).ShouldBeTrue();
        (await stateMachineHarness.Exists(correllationId,
             stateMachineHarness.StateMachine.BalanceUnDeductFail)).ShouldNotBeNull();
    }

    [Fact]
    public async Task WhenWithdrawalStatusRequested_ShouldReturnNotFound()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureTestMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
         (TonWithdrawFlowState.AwaitingDeduction));

        var requestClient = harness.GetRequestClient<ITonWithdrawalStatusRequested>();

        // act
        var response = await requestClient.GetResponse<TonWithdrawalStatus, TonWithdrawalNotfound>(new
        {
            CorrelationId = Guid.NewGuid()
        });

        // assert
        response.Is(out Response<TonWithdrawalNotfound> _).ShouldBeTrue();
    }

    [Fact]
    public async Task WhenWithdrawalStatusRequested_ShouldReturnStatus()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureTestMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();
        var saga = CreateSagaWithState
            (TonWithdrawFlowState.AwaitingDeduction);
        await provider.AddOrUpdateSaga(saga);

        var stateMachineHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();
        var requestClient = harness.GetRequestClient<ITonWithdrawalStatusRequested>();

        // act
        var response = await requestClient.GetResponse<TonWithdrawalStatus, TonWithdrawalNotfound>(new
        {
            CorrelationId = correllationId
        });

        // assert
        response.Is(out Response<TonWithdrawalStatus> status).ShouldBeTrue();

        status.Message.destination.ShouldBe(saga.Destination);
        status.Message.failReason.ShouldBeNull();
        status.Message.nanotons.ShouldBe(saga.Nanotons);
    }


    [Fact]
    public async Task WhenSendTonsConfirmationPeriodExpired()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureTestMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (TonWithdrawFlowState.AwaitingDeduction));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();

        await harness.Bus.Publish<IBalanceDeducted>(new BalanceDeductedImpl()
        {
            CorrelationId = correllationId
        });

        await sagaHarness.Exists(correllationId, x => x.BalanceDeductedSuccess);

        // act
        using var adjustment = new QuartzTimeAdjustment(provider);
        await adjustment.AdvanceTime(TimeSpan.FromMinutes(10));

        // assert
        (await sagaHarness.Exists(correllationId, x => x.Expired))
            .HasValue.ShouldBeTrue();

        await harness.OutputTimeline(_output, x => x.Now());
    }
}

#region Contract Implementations
public class BalanceDeductedImpl : IBalanceDeducted
{
    public Guid CorrelationId { get; set; }
}

public class DeductBalanceImpl : IDeductBalance
{
    public Guid CorrelationId { get; set; }
    public Guid UserId { get; set; }
    public FundsInfo Funds { get; set; }
    public NegativeBalanceOperationType Type { get; set; }
}

public class UnDeductBalanceImpl : IUnDeductBalance
{
    public Guid CorrelationId { get; set; }
}

public class SendTonsCompletedImpl : ISendTonsCompleted
{
    public Guid CorrelationId { get; set; }
}
#endregion