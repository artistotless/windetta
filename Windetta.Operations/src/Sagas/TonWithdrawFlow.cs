using MassTransit;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Contracts.Queries;

namespace Windetta.Operations.Sagas;

public class TonWithdrawFlow : SagaStateMachineInstance
{
    public Guid UserId { get; set; }
    public long Nanotons { get; set; }
    public TonAddress Destination { get; set; }
    public int CurrentState { get; set; }
    public Guid CorrelationId { get; set; }
    public string? FailReason { get; set; }
    public Guid? ExpirationTokenId { get; set; }
}

public class TonWithdrawFlowStateMachine : MassTransitStateMachine<TonWithdrawFlow>
{
    public TonWithdrawFlowStateMachine()
    {
        Event(() => WithdrawRequested, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => BalanceDeducted, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => TransferTonCompleted, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => BalanceDeductFailed, x => x.CorrelateById(ctx => ctx.Message.Message.CorrelationId));
        Event(() => TransferTonConfirmationPeriodExpired, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => WithdrawalStatusRequested, x =>
        {
            x.CorrelateById(ctx => ctx.Message.CorrelationId);
            x.ReadOnly = true;
        });

        Schedule(() => ExpirationSchedule, x => x.ExpirationTokenId, x => x.Delay = TimeSpan.FromMinutes(5));

        InstanceState(instance => instance.CurrentState);

        Initially(
            When(WithdrawRequested)
                .CopyDataToInstance()
                .TransitionTo(AwaitingDeduction)
                .DeductFromBalance());

        During(AwaitingDeduction,
            When(BalanceDeducted)
                .TransitionTo(BalanceDeductedSuccess)
                .TransferTon()
                .Schedule(ExpirationSchedule,
                ctx => ctx.Init<ITransferTonConfirmationPeriodExpired>(new { CorrelationId = ctx.Saga.CorrelationId })),
            When(BalanceDeductFailed)
                .Then(ctx => { ctx.Saga.FailReason = ctx.Message.Exceptions.FirstOrDefault()?.Message; })
                .Unschedule(ExpirationSchedule)
                .TransitionTo(BalanceDeductFail));

        During(BalanceDeductedSuccess,
            When(TransferTonFailed)
                .Unschedule(ExpirationSchedule)
                .Then(ctx => { ctx.Saga.FailReason = ctx.Message.Exceptions.FirstOrDefault()?.Message; })
                .UnDeductFromBalance()
                .TransitionTo(TransferTonFail));

        DuringAny(
            When(TransferTonConfirmationPeriodExpired)
                .PublishAsync(ctx => ctx.Init<ITransferTonConfirmationPeriodExpired>(new
                {
                    CorrelationId = ctx.Message.CorrelationId
                }))
                .TransitionTo(Expired));

        DuringAny(
            When(TransferTonCompleted)
                .Unschedule(ExpirationSchedule)
                .Finalize());

        DuringAny(
            When(WithdrawalStatusRequested)
                .ReturnStatus());
    }

    public State AwaitingDeduction { get; set; }
    public State BalanceDeductedSuccess { get; set; }
    public State BalanceDeductFail { get; set; }
    public State TransferTonFail { get; set; }
    public State Expired { get; set; }

    public Event<IWithdrawTonRequested> WithdrawRequested { get; }
    public Event<IBalanceDeducted> BalanceDeducted { get; }
    public Event<Fault<IDeductBalance>> BalanceDeductFailed { get; }
    public Event<ITransferTonCompleted> TransferTonCompleted { get; }
    public Event<Fault<ITransferTon>> TransferTonFailed { get; }
    public Event<ITransferTonConfirmationPeriodExpired> TransferTonConfirmationPeriodExpired { get; }
    public Event<IGetTonWithdrawalStatus> WithdrawalStatusRequested { get; }

    public Schedule<TonWithdrawFlow, ITransferTonConfirmationPeriodExpired> ExpirationSchedule { get; }
}

public static class BalanceWithdrawFlowStateMachineExtensions
{
    public static EventActivityBinder<TonWithdrawFlow, IWithdrawTonRequested> CopyDataToInstance(
        this EventActivityBinder<TonWithdrawFlow, IWithdrawTonRequested> binder)
    {
        return binder.Then(ctx =>
        {
            ctx.Saga.UserId = ctx.Message.UserId;
            ctx.Saga.CorrelationId = ctx.Message.CorrelationId;
            ctx.Saga.Nanotons = ctx.Message.Nanotons;
            ctx.Saga.Destination = ctx.Message.Destination;
        });
    }

    public static EventActivityBinder<TonWithdrawFlow, IWithdrawTonRequested> DeductFromBalance(
        this EventActivityBinder<TonWithdrawFlow, IWithdrawTonRequested> binder)
    {
        var endpoint = new MyEndpointNameFormatter(Svc.Wallet)
            .CommandUri<IDeductBalance>();

        return binder.SendAsync(endpoint, ctx => ctx.Init<IDeductBalance>(new
        {
            Amount = ctx.Message.Nanotons,
            CorrelationId = ctx.Message.CorrelationId,
            UserId = ctx.Message.UserId,
        }));
    }

    public static EventActivityBinder<TonWithdrawFlow, T> UnDeductFromBalance<T>(
     this EventActivityBinder<TonWithdrawFlow, T> binder) where T : class
    {
        var endpoint = new MyEndpointNameFormatter(Svc.Wallet)
           .CommandUri<IUnDeductBalance>();

        return binder.SendAsync(endpoint, ctx => ctx.Init<IUnDeductBalance>(new
        {
            CorrelationId = ctx.Saga.CorrelationId,
            UserId = ctx.Saga.UserId,
            Amount = ctx.Saga.Nanotons,
        }));
    }

    public static EventActivityBinder<TonWithdrawFlow, T> TransferTon<T>(
    this EventActivityBinder<TonWithdrawFlow, T> binder) where T : class
    {
        var endpoint = new MyEndpointNameFormatter(Svc.TonTxns)
           .CommandUri<ITransferTon>();

        return binder.SendAsync(endpoint, ctx => ctx.Init<ITransferTon>(new
        {
            CorrelationId = ctx.Saga.CorrelationId,
            Destination = new TonAddress(ctx.Saga.Destination),
            Nanotons = ctx.Saga.Nanotons,
        }));
    }

    public static EventActivityBinder<TonWithdrawFlow, T> ReturnStatus<T>(
    this EventActivityBinder<TonWithdrawFlow, T> binder) where T : class
    {
        return binder.Respond(ctx => new TonWithdrawalStatus(
            ctx.Saga.Nanotons,
            ctx.Saga.CurrentState,
            ctx.Saga.FailReason));
    }
}