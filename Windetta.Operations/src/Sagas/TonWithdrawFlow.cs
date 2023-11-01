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
        Event(() => TransferTonCompleted, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => BalanceDeductFailed, x => x.CorrelateById(ctx => ctx.Message.Message.CorrelationId));
        Event(() => WithdrawPeriodExpired, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

        Schedule(() => ExpirationSchedule, x => x.ExpirationTokenId, x => x.Delay = TimeSpan.FromMinutes(5));

        InstanceState(instance => instance.CurrentState);

        Initially(
            When(WithdrawRequested)
                .CopyDataToInstance()
                .TransitionTo(AwaitingDeduction)
                .DeductFromBalance()
                );

        During(AwaitingDeduction,
            When(BalanceDeducted)
                .TransitionTo(BalanceDeductedSuccess)
                .TransferTon()
                .Schedule(ExpirationSchedule,
                ctx => ctx.Init<IWithdrawPeriodExpired>(new { CorrelationId = ctx.Saga.CorrelationId })),
            When(BalanceDeductFailed)
                .Then(ctx => { ctx.Saga.FailReason = ctx.Message.Exceptions.FirstOrDefault()?.Message; })
                .Unschedule(ExpirationSchedule)
                .TransitionTo(BalanceDeductFail));

        DuringAny(
            When(WithdrawPeriodExpired)
                .UnDeductFromBalance()
                .TransitionTo(Expired));

        DuringAny(
           When(TransferTonCompleted)
               .Unschedule(ExpirationSchedule)
               .Finalize());

        During(BalanceDeductedSuccess,
          When(TransferTonFailed)
              .Unschedule(ExpirationSchedule)
              .UnDeductFromBalance()
              .TransitionTo(TransferTonFail));
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
    public Event<IWithdrawPeriodExpired> WithdrawPeriodExpired { get; }

    public Schedule<TonWithdrawFlow, IWithdrawPeriodExpired> ExpirationSchedule { get; }
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
        var address = new MyEndpointNameFormatter(Svc.Wallet).CommandUri<IDeductBalance>();

        return binder.SendAsync(new Uri($"queue:{address}"), ctx => ctx.Init<IDeductBalance>(new
        {
            Amount = ctx.Message.Nanotons,
            CorrelationId = ctx.Message.CorrelationId,
            UserId = ctx.Message.UserId,
        }));

        //return binder.PublishAsync(context => context.Init<IDeductBalance>(new
        //{
        //    Amount = context.Message.Nanotons,
        //    CorrelationId = context.Message.CorrelationId,
        //    UserId = context.Message.UserId,
        //}));
    }

    public static EventActivityBinder<TonWithdrawFlow, T> UnDeductFromBalance<T>(
     this EventActivityBinder<TonWithdrawFlow, T> binder) where T : class
    {
        return binder.SendAsync(context => context.Init<IUnDeductBalance>(new
        {
            CorrelationId = context.Saga.CorrelationId,
            UserId = context.Saga.UserId,
            Amount = context.Saga.Nanotons,
        }));
    }

    public static EventActivityBinder<TonWithdrawFlow, T> TransferTon<T>(
    this EventActivityBinder<TonWithdrawFlow, T> binder) where T : class
    {
        return binder.SendAsync(context => context.Init<ITransferTon>(new
        {
            CorrelationId = context.Saga.CorrelationId,
            Destination = new TonAddress(context.Saga.Destination),
            Nanotons = context.Saga.Nanotons,
        }));
    }
}

public class SomeConsumer : IConsumer<IDeductBalance>
{
    public Task Consume(ConsumeContext<IDeductBalance> context)
    {
        throw new NotImplementedException();
    }
}

public class SomeBatchConsumer : IConsumer<Batch<ITransferTon>>
{
    public Task Consume(ConsumeContext<Batch<ITransferTon>> context)
    {
        throw new NotImplementedException();
    }
}