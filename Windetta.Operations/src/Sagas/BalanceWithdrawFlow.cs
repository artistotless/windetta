using MassTransit;
using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;

namespace Windetta.Operations.Sagas;

public class BalanceWithdrawFlow : SagaStateMachineInstance
{
    public Guid UserId { get; set; }
    public long Nanotons { get; set; }
    public TonAddress Destination { get; set; }
    public int CurrentState { get; set; }
    public Guid CorrelationId { get; set; }
}

public class BalanceWithdrawFlowStateMachine : MassTransitStateMachine<BalanceWithdrawFlow>
{
    public BalanceWithdrawFlowStateMachine()
    {
        Event(() => WithdrawRequested, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => BalanceDeducted, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => TransferAddedToPool, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => TransferTonCompleted, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

        InstanceState(instance => instance.CurrentState);

        Initially(
            When(WithdrawRequested)
            .Then(ctx =>
            {
                ctx.Saga.UserId = ctx.Message.UserId;
                ctx.Saga.CorrelationId = ctx.Message.CorrelationId;
                ctx.Saga.Nanotons = ctx.Message.Nanotons;
                ctx.Saga.Destination = ctx.Message.Destination;
            })
            .DeductFromBalance()
            .TransitionTo(AwaitingDeduction));

        During(AwaitingDeduction,
            When(BalanceDeducted)
            .TransitionTo(BalanceDeductedSuccess)
            .AddTransferToPool());

        During(BalanceDeductedSuccess,
            When(TransferAddedToPool)
            .TransitionTo(TransferTonPlanned));

        DuringAny(
            When(TransferTonCompleted)
            .Finalize());
    }

    public State AwaitingDeduction { get; set; }
    public State BalanceDeductedSuccess { get; set; }
    public State BalanceDeductFailed { get; set; }
    public State TransferTonPlanned { get; set; }
    public State WithdrawPeriodExpired { get; set; }

    public Event<IWithdrawTonRequested> WithdrawRequested { get; }
    public Event<IBalanceDeducted> BalanceDeducted { get; }
    public Event<ITransferTonAddedToPool> TransferAddedToPool { get; }
    public Event<ITransferTonCompleted> TransferTonCompleted { get; }
}

public static class BalanceWithdrawFlowStateMachineExtensions
{
    public static EventActivityBinder<BalanceWithdrawFlow, T> DeductFromBalance<T>(
       this EventActivityBinder<BalanceWithdrawFlow, T> binder) where T : class
    {
        return binder.SendAsync(context => context.Init<IDeductBalance>(new
        {
            CorrelationId = context.Saga.CorrelationId,
            UserId = context.Saga.UserId,
            Amount = context.Saga.Nanotons,

        }));
    }

    public static EventActivityBinder<BalanceWithdrawFlow, T> AddTransferToPool<T>(
    this EventActivityBinder<BalanceWithdrawFlow, T> binder) where T : class
    {
        return binder.SendAsync(context => context.Init<IDeductBalance>(new
        {
            CorrelationId = context.Saga.CorrelationId,
            UserId = context.Saga.UserId,
            Amount = context.Saga.Nanotons,
        }));
    }
}