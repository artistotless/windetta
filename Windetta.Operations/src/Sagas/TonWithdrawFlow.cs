using MassTransit;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;

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
        Event(() => BalanceUnDeductFailed, x => x.CorrelateById(ctx => ctx.Message.Message.CorrelationId));
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
                .DeductFromBalance()
                .TransitionTo(AwaitingDeduction));

        During(AwaitingDeduction,
            When(BalanceDeducted)
                .TransferTon()
                .Schedule(ExpirationSchedule,
                ctx => ctx.Init<ITransferTonConfirmationPeriodExpired>
                (new { CorrelationId = ctx.Saga.CorrelationId }))
                .TransitionTo(BalanceDeductedSuccess),
            When(BalanceDeductFailed)
                .SaveError()
                .TransitionTo(BalanceDeductFail));

        During(BalanceDeductedSuccess,
            When(TransferTonCompleted)
                .Unschedule(ExpirationSchedule)
                .Finalize(),
            When(TransferTonFailed)
                .Unschedule(ExpirationSchedule)
                .SaveError()
                .UnDeductFromBalance()
                .TransitionTo(TransferTonFail),
            When(TransferTonConfirmationPeriodExpired)
                .NotifyWithdrawalExpired()
                .TransitionTo(Expired));

        During(TransferTonFail,
            When(BalanceUnDeductFailed)
                .NotifyUnDeductBalanceFailed()
                .TransitionTo(BalanceUnDeductFail));

        DuringAny(
            When(WithdrawalStatusRequested)
                .ReturnStatus());
    }

    public State AwaitingDeduction { get; set; }
    public State BalanceDeductedSuccess { get; set; }
    public State BalanceDeductFail { get; set; }
    public State BalanceUnDeductFail { get; set; }
    public State TransferTonFail { get; set; }
    public State Expired { get; set; }

    public Event<IWithdrawTonRequested> WithdrawRequested { get; }
    public Event<IBalanceDeducted> BalanceDeducted { get; }
    public Event<Fault<IDeductBalance>> BalanceDeductFailed { get; }
    public Event<Fault<IUnDeductBalance>> BalanceUnDeductFailed { get; }
    public Event<ISendTonsCompleted> TransferTonCompleted { get; }
    public Event<Fault<ISendTons>> TransferTonFailed { get; }
    public Event<ITransferTonConfirmationPeriodExpired> TransferTonConfirmationPeriodExpired { get; }
    public Event<ITonWithdrawalStatusRequested> WithdrawalStatusRequested { get; }

    public Schedule<TonWithdrawFlow, ITransferTonConfirmationPeriodExpired> ExpirationSchedule { get; }
}

public class TonWithdrawFlowDefinition : SagaDefinition<TonWithdrawFlow>
{
    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<TonWithdrawFlow> sagaConfigurator,
        IRegistrationContext context)
    {
        sagaConfigurator.UseInMemoryOutbox(context);
    }
}

#region Extension methods
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
        return binder.SendCommandAsync(Svc.Wallet, ctx => ctx.Init<IDeductBalance>(new
        {
            Amount = ctx.Message.Nanotons,
            CorrelationId = ctx.Message.CorrelationId,
            UserId = ctx.Message.UserId,
        }));
    }

    public static EventActivityBinder<TonWithdrawFlow, T> UnDeductFromBalance<T>(
     this EventActivityBinder<TonWithdrawFlow, T> binder) where T : class
    {
        return binder.SendCommandAsync(Svc.Wallet, ctx => ctx.Init<IUnDeductBalance>(new
        {
            CorrelationId = ctx.Saga.CorrelationId,
            UserId = ctx.Saga.UserId,
            Amount = ctx.Saga.Nanotons,
        }));
    }

    public static EventActivityBinder<TonWithdrawFlow, T> TransferTon<T>(
    this EventActivityBinder<TonWithdrawFlow, T> binder) where T : class
    {
        return binder.SendCommandAsync(Svc.TonTxns, ctx => ctx.Init<ISendTons>(new
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
            ctx.Saga.Destination,
            ctx.Saga.FailReason));
    }

    public static EventActivityBinder<TonWithdrawFlow, T> SaveError<T>(
    this EventActivityBinder<TonWithdrawFlow, T> binder) where T : class, Fault
    {
        return binder.Then(ctx =>
        {
            ctx.Saga.FailReason = ctx.Message.Exceptions.FirstOrDefault()?.Message;
        });
    }

    public static EventActivityBinder<TonWithdrawFlow, T> NotifyWithdrawalExpired<T>(
    this EventActivityBinder<TonWithdrawFlow, T> binder) where T : class
    {
        return binder.SendCommandAsync(Svc.Notifications, ctx => ctx.Init<INotifyTonWithdrawalExpired>(new
        {
            CorrelationId = ctx.Saga.CorrelationId,
            Destination = ctx.Saga.Destination,
            Nanotons = ctx.Saga.Nanotons,
            UserId = ctx.Saga.UserId
        }));
    }

    public static EventActivityBinder<TonWithdrawFlow, T> NotifyUnDeductBalanceFailed<T>(
    this EventActivityBinder<TonWithdrawFlow, T> binder) where T : class
    {
        return binder.SendCommandAsync(Svc.Notifications, ctx => ctx.Init<INotifyUnDeductBalanceFailed>(new
        {
            CorrelationId = ctx.Saga.CorrelationId,
            UserId = ctx.Saga.UserId
        }));
    }
    #endregion
}