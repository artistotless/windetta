using MassTransit;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;

namespace Windetta.TonTxns.Infrastructure.Sagas;

public class TonWithdrawFlow : SagaStateMachineInstance
{
    public Guid UserId { get; set; }
    public ulong Nanotons { get; set; }
    public TonAddress Destination { get; set; }
    public string CurrentState { get; set; }
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
        Event(() => SendTonsCompleted, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => BalanceDeductFailed, x => x.CorrelateById(ctx => ctx.Message.Message.CorrelationId));
        Event(() => BalanceUnDeductFailed, x => x.CorrelateById(ctx => ctx.Message.Message.CorrelationId));
        Event(() => SendTonsConfirmationPeriodExpired, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => WithdrawalStatusRequested, x =>
        {
            x.OnMissingInstance(m => m.ExecuteAsync(
                a => a.RespondAsync(new TonWithdrawalNotfound())));

            x.CorrelateById(ctx => ctx.Message.CorrelationId);
            x.ReadOnly = true;
        });

        Schedule(() => ExpirationSchedule,
            x => x.ExpirationTokenId,
            x => x.Delay = TimeSpan.FromMinutes(10));

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
                ctx => ctx.Init<ISendTonsConfirmationPeriodExpired>
                (new { ctx.Message.CorrelationId }))
                .TransitionTo(BalanceDeductedSuccess),
            When(BalanceDeductFailed)
                .SaveError()
                .TransitionTo(BalanceDeductFail));

        During(BalanceDeductedSuccess,
            When(SendTonsCompleted)
                .Unschedule(ExpirationSchedule)
                .Finalize(),
            When(SendTonsFailed)
                .Unschedule(ExpirationSchedule)
                .SaveError()
                .UnDeductFromBalance()
                .TransitionTo(SendTonsFail),
            When(SendTonsConfirmationPeriodExpired)
                .NotifyWithdrawalExpired()
                .TransitionTo(Expired));

        During(SendTonsFail,
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
    public State SendTonsFail { get; set; }
    public State Expired { get; set; }

    public Event<IWithdrawTonRequested> WithdrawRequested { get; }
    public Event<IBalanceDeducted> BalanceDeducted { get; }
    public Event<Fault<IDeductBalance>> BalanceDeductFailed { get; }
    public Event<Fault<IUnDeductBalance>> BalanceUnDeductFailed { get; }
    public Event<ISendTonsCompleted> SendTonsCompleted { get; }
    public Event<Fault<ISendTons>> SendTonsFailed { get; }
    public Event<ISendTonsConfirmationPeriodExpired> SendTonsConfirmationPeriodExpired { get; }
    public Event<ITonWithdrawalStatusRequested> WithdrawalStatusRequested { get; }

    public Schedule<TonWithdrawFlow, ISendTonsConfirmationPeriodExpired> ExpirationSchedule { get; }
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
        return binder.SendCommandAsync(Svc.Wallet,
            ctx => ctx.Init<IDeductBalance>(new
            {
                Amount = ctx.Message.Nanotons,
                CurrencyId = (int)Currencies.Ton,
                ctx.Message.CorrelationId,
                ctx.Message.UserId,
            }));
    }

    public static EventActivityBinder<TonWithdrawFlow, T> UnDeductFromBalance<T>(
     this EventActivityBinder<TonWithdrawFlow, T> binder) where T : class
    {
        return binder.SendCommandAsync(Svc.Wallet,
            ctx => ctx.Init<IUnDeductBalance>(new
            {
                ctx.Saga.CorrelationId,
            }));
    }

    public static EventActivityBinder<TonWithdrawFlow, T> TransferTon<T>(
    this EventActivityBinder<TonWithdrawFlow, T> binder) where T : class
    {
        return binder.SendCommandAsync(Svc.TonTxns,
            ctx => ctx.Init<ISendTons>(new
            {
                ctx.Saga.CorrelationId,
                Destination = new TonAddress(ctx.Saga.Destination),
                ctx.Saga.Nanotons,
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
            ctx.Saga.FailReason = ctx.Message.Exceptions?
            .FirstOrDefault()?.Message;
        });
    }

    public static EventActivityBinder<TonWithdrawFlow, T> NotifyWithdrawalExpired<T>(
    this EventActivityBinder<TonWithdrawFlow, T> binder) where T : class
    {
        return binder.SendCommandAsync(Svc.Notifications,
            ctx => ctx.Init<INotifyTonWithdrawalExpired>(new
            {
                ctx.Saga.CorrelationId,
                ctx.Saga.Destination,
                ctx.Saga.Nanotons,
                ctx.Saga.UserId
            }));
    }

    public static EventActivityBinder<TonWithdrawFlow, T> NotifyUnDeductBalanceFailed<T>(
    this EventActivityBinder<TonWithdrawFlow, T> binder) where T : class
    {
        return binder.SendCommandAsync(Svc.Notifications,
            ctx => ctx.Init<INotifyUnDeductBalanceFailed>(new
            {
                ctx.Saga.CorrelationId,
                ctx.Saga.UserId
            }));
    }
    #endregion
}