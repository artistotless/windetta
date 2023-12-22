using MassTransit;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Infrastructure.Sagas.Activities;

namespace Windetta.Main.Infrastructure.Sagas;

public class MatchFlow : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public IEnumerable<Player> Players { get; set; }
    public Guid GameId { get; set; }
    public string Endpoint { get; set; }
    public DateTimeOffset Created { get; set; }
    public Bet Bet { get; set; }
    public int CurrentState { get; set; }
    public string? CanceledReason { get; set; }
}

public enum MatchFlowState : int
{
    AwaitingHoldBalances = 3,
    GameServerSearch = 4,
    GameServerSearchExpired = 5,
    Running = 6,
    ProcessingWinnings = 7,
    ProcessingWinningsFail = 8,
    Canceled = 9,
}

public class MatchFlowStateMachine : MassTransitStateMachine<MatchFlow>
{
    public MatchFlowStateMachine(IMatchHubs hubs)
    {
        InstanceState(instance => instance.CurrentState);

        Initially(
            When(MatchHubReady)
                .CopyDataToInstance(hubs)
                .HoldBalances()
                .TransitionTo(AwaitingHoldBalances));

        During(AwaitingHoldBalances,
            When(BalancesHeld)
                .TransitionTo(GameServerSearch)
                .StartGameServerSearching(),
            When(HoldBalancesFailed)
                .NotifyMatchCanceled(x => "One of the players has insufficient balance")
                .Finalize());

        During(GameServerSearch,
            Ignore(MatchHubReady),
            When(GameServerReservationFailed)
                .NotifyMatchCanceled(x => "GameServers unavailable")
                .Finalize(),
            When(GameServerPrepared)
                .NotifyMatchBegun()
                .TransitionTo(Running),
            When(GameServerReservationPeriodExpired)
                .TransitionTo(GameServerSearchExpired)
                .NotifyMatchAwaitingExpired(),
            When(CancellationRequested)
                .SetCanceledReason(x => x.Reason)
                .TransitionTo(Canceled));

        During(Running,
            When(CancellationRequested)
                .SetCanceledReason(x => x.Reason)
                .TransitionTo(Canceled),
            When(MatchCompleted)
                .TransitionTo(ProcessingWinnings)
                .ProcessWinnings());

        During(ProcessingWinnings,
            When(WinningsProcessed)
                .Finalize(),
            When(ProcessingWinningsFailed)
                .TransitionTo(ProcessingWinningsFail));

        WhenEnter(Final, x =>
        {
            // TODO: add match to archive/history service
            return x;
        });

        SetCompletedWhenFinalized();
    }

    public State AwaitingHoldBalances { get; set; }
    public State GameServerSearch { get; set; }
    public State GameServerSearchExpired { get; set; }
    public State Running { get; set; }
    public State ProcessingWinnings { get; set; }
    public State ProcessingWinningsFail { get; set; }
    public State Canceled { get; set; }

    public Event<IMatchHubReady> MatchHubReady { get; }
    public Event<IBalancesHeld> BalancesHeld { get; }
    public Event<IWinningsProcessed> WinningsProcessed { get; }
    public Event<IGameServerReservationPeriodExpired> GameServerReservationPeriodExpired { get; }
    public Event<IGameServerPrepared> GameServerPrepared { get; }
    public Event<ICancellationMatchRequested> CancellationRequested { get; }
    public Event<IMatchCompleted> MatchCompleted { get; }

    // Faults
    public Event<Fault<IHoldBalances>> HoldBalancesFailed { get; }
    public Event<Fault<IBalancesHeld>> GameServerReservationFailed { get; }
    public Event<Fault<IMatchCompleted>> ProcessingWinningsFailed { get; }

}

public class MatchFlowDefinition : SagaDefinition<MatchFlow>
{
    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<MatchFlow> sagaConfigurator,
        IRegistrationContext context)
    {
        sagaConfigurator.UseInMemoryOutbox(context);
        sagaConfigurator.UseMessageRetry(c =>
        {
            c.Interval(10, TimeSpan.FromSeconds(10));
            c.Ignore<LspmException>(e => e.ErrorCode == Errors.LSPM.NotFound);
        });
    }
}

#region Extension methods
public static class MatchFlowStateMachineExtensions
{
    public static EventActivityBinder<MatchFlow, IMatchHubReady> CopyDataToInstance(
        this EventActivityBinder<MatchFlow, IMatchHubReady> binder, IMatchHubs hubs)
    {

        return binder.Then(async ctx =>
        {
            var hub = await hubs.GetAsync(ctx.Message.CorrelationId);

            if (hub is null)
                throw MatchHubException.NotFound;

            ctx.Saga.Bet = hub.Bet;
            ctx.Saga.GameId = hub.GameId;
            ctx.Saga.Players = hub.Rooms.SelectMany(r => r.Members,
                (r, m) => new Player(m.Id, "Nick", r.Index));
            ctx.Saga.CorrelationId = ctx.Message.CorrelationId;
            ctx.Saga.Created = ctx.Message.TimeStamp;
        });
    }

    public static EventActivityBinder<MatchFlow, IBalancesHeld> StartGameServerSearching(
    this EventActivityBinder<MatchFlow, IBalancesHeld> binder)
    {
        return binder.Activity(x => x.OfInstanceType<SearchingGameServerActivity>());
    }

    public static EventActivityBinder<MatchFlow, IMatchHubReady> HoldBalances(
    this EventActivityBinder<MatchFlow, IMatchHubReady> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<IHoldBalances>(new
        {
            ctx.Message.CorrelationId,
            Funds = new FundsInfo(ctx.Saga.Bet.CurrencyId, ctx.Saga.Bet.Amount),
            UsersIds = ctx.Saga.Players.Select(x => x.Id)
        }));
    }

    public static EventActivityBinder<MatchFlow, IGameServerPrepared> NotifyMatchBegun(
    this EventActivityBinder<MatchFlow, IGameServerPrepared> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyMatchBegun>(new
        {
            ctx.Message.CorrelationId,
            ctx.Message.Endpoint,
            ctx.Message.Tickets
        }));
    }

    public static EventActivityBinder<MatchFlow, TData> NotifyMatchCanceled<TData>(
    this EventActivityBinder<MatchFlow, TData> binder, Func<TData, string> selectReason)
    where TData : class
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyMatchCanceled>(new
        {
            ctx.Saga.CorrelationId,
            Reason = selectReason.Invoke(ctx.Message)
        }));
    }

    public static EventActivityBinder<MatchFlow, Fault<IMatchCompleted>> NotifyProccessingWinningsFailed(
    this EventActivityBinder<MatchFlow, Fault<IMatchCompleted>> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyProccessingWinningsFailed>(new
        {
            ctx.Message.Message.CorrelationId,
            FaultMessage = ctx.Message
        }));
    }

    public static EventActivityBinder<MatchFlow, TData> SetCanceledReason<TData>(
    this EventActivityBinder<MatchFlow, TData> binder, Func<TData, string> selectReason)
    where TData : class
    {
        return binder.Then(x => x.Saga.CanceledReason = selectReason(x.Message));
    }

    public static EventActivityBinder<MatchFlow, IMatchCompleted> ProcessWinnings(
    this EventActivityBinder<MatchFlow, IMatchCompleted> binder)
    {
        return binder.Activity(x => x.OfType<ProcessingWinningsActivity>());
    }

    public static EventActivityBinder<MatchFlow, IGameServerReservationPeriodExpired> NotifyMatchAwaitingExpired(
    this EventActivityBinder<MatchFlow, IGameServerReservationPeriodExpired> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyMatchAwaitingExpired>(new
        {
            ctx.Message.CorrelationId,
        }));
    }
    #endregion
}
