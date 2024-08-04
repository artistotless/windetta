using MassTransit;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;

namespace Windetta.Main.Infrastructure.Sagas;

public class ProcessingWinningsFlow : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public IEnumerable<Guid> Winners { get; set; }
    public IEnumerable<Guid> Losers { get; set; }
    public int BetCurrencyId { get; set; }
    public ulong BetAmount { get; set; }
    public DateTimeOffset Created { get; set; }
    public int CurrentState { get; set; }
}

public enum ProcessingWinningsFlowState : int
{
    ProcessingWinnings = 3,
    ProcessingWinningsSuccess = 4,
    ProcessingWinningsFail = 5,
}

public class ProcessingWinningsFlowStateMachine : MassTransitStateMachine<ProcessingWinningsFlow>
{
    public ProcessingWinningsFlowStateMachine()
    {
        InstanceState(instance => instance.CurrentState);

        Initially(
            When(FlowCreated)
                .CopyDataToInstance()
                .TransitionTo(ProcessingWinnings));

        During(ProcessingWinnings,
            When(WinningsProcessed)
                .Finalize(),
            When(IncreaseBalancesFailed)
                .NotifyProccessingWinningsFailed("IncreaseBalancesFailed")
                .TransitionTo(ProcessingWinningsFail),
            When(DeductUnHolBalancesFailed)
                .NotifyProccessingWinningsFailed("DeductUnHolBalancesFailed")
                .TransitionTo(ProcessingWinningsFail));

        WhenEnter(Final, x =>
        {
            // TODO: add match to archive/history service
            return x;
        });

        SetCompletedWhenFinalized();
    }

    public State ProcessingWinnings { get; set; }
    public State ProcessingWinningsSuccess { get; set; }
    public State ProcessingWinningsFail { get; set; }

    public Event<ICreateWinningsFlowRequested> FlowCreated { get; }
    public Event<IBalanceIncreased> WinningsProcessed { get; }

    // Faults
    public Event<Fault<IDeductUnHoldBalance>> DeductUnHolBalancesFailed { get; }
    public Event<Fault<IIncreaseBalance>> IncreaseBalancesFailed { get; }
}

public class ProcessingWinningsFlowDefinition : SagaDefinition<ProcessingWinningsFlow>
{
    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<ProcessingWinningsFlow> sagaConfigurator,
        IRegistrationContext context)
    {
        sagaConfigurator.UseInMemoryOutbox(context);
        sagaConfigurator.UseMessageRetry(c =>
        {
            c.Interval(6, TimeSpan.FromSeconds(10));
        });
    }
}

#region Extension methods
public static class ProcessingWinningsFlowStateMachineExtensions
{
    public static EventActivityBinder<ProcessingWinningsFlow, ICreateWinningsFlowRequested> CopyDataToInstance(
   this EventActivityBinder<ProcessingWinningsFlow, ICreateWinningsFlowRequested> binder)
    {
        return binder.Then(ctx =>
        {
            ctx.Saga.BetCurrencyId = ctx.Message.Funds.CurrencyId;
            ctx.Saga.BetAmount = ctx.Message.Funds.Amount;
            ctx.Saga.Winners = ctx.Message.Winners;
            ctx.Saga.Losers = ctx.Message.Losers;
            ctx.Saga.CorrelationId = ctx.Message.CorrelationId;
            ctx.Saga.Created = DateTime.UtcNow;
        });
    }

    public static EventActivityBinder<ProcessingWinningsFlow, ICreateWinningsFlowRequested> ProcessWinnings(
        this EventActivityBinder<ProcessingWinningsFlow, ICreateWinningsFlowRequested> binder)
    {
        return binder
          .SendCommandAsync(Svc.Wallet, ctx => ctx.Init<IDeductUnHoldBalance>(new
          {
              ctx.Message.CorrelationId,
              Type = NegativeBalanceOperationType.Loss,
              Data = ctx.Message.Losers.Select(id => new BalanceOperationData()
              {
                  Funds = ctx.Message.Funds,
                  UserId = id,
                  OperationId = Guid.NewGuid(),
              }),
          }))
          .SendCommandAsync(Svc.Wallet, ctx => ctx.Init<IIncreaseBalance>(new
          {
              ctx.Message.CorrelationId,
              Type = PositiveBalanceOperationType.Winnings,
              Data = ctx.Message.Winners.Select(id => new BalanceOperationData()
              {
                  Funds = ctx.Message.Funds,
                  UserId = id,
                  OperationId = Guid.NewGuid(),
              }),
          })).SendCommandAsync(Svc.Wallet, ctx => ctx.Init<IUnHoldBalances>(new
          {
              ctx.Message.CorrelationId,
              ctx.Message.Funds,
              UsersIds = ctx.Message.Winners
          }))
          .Finalize();
    }

    public static EventActivityBinder<ProcessingWinningsFlow, T> NotifyProccessingWinningsFailed<T>(
        this EventActivityBinder<ProcessingWinningsFlow, T> binder, string message) where T : class
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyProccessingWinningsFailed>(new
        {
            ctx.Saga.CorrelationId,
            FaultMessage = message
        }));
    }
    #endregion
}
