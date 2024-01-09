using LSPM.Models;
using MassTransit;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.MatchHubs;

namespace Windetta.Main.Infrastructure.Sagas;

public class MatchFlow : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public IEnumerable<Player> Players { get; set; }
    public IReadOnlyDictionary<Guid, string>? Tickets { get; set; }
    public IReadOnlyDictionary<string, string>? Properties { get; set; }
    public Guid GameId { get; set; }
    public string? Endpoint { get; set; }
    public DateTimeOffset Created { get; set; }
    public int BetCurrencyId { get; set; }
    public ulong BetAmount { get; set; }
    public int CurrentState { get; set; }
    public string? CanceledReason { get; set; }
}

public enum MatchFlowState : int
{
    AwaitingHoldBalances = 3,
    GameServerSearching = 4,
    GameServerSearchExpired = 5,
    Running = 6,
    ProcessingWinnings = 7,
    ProcessingWinningsFail = 8,
    ServerFound = 9,
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
                .StartGameServerSearching()
                .TransitionTo(GameServerSearching),
            When(HoldBalancesFailed)
                .CancelMatch(reason => "One of the players has insufficient balance"));

        During(GameServerSearching,
            When(GameServerReservationFailed)
                .CancelMatch(reason => "GameServers unavailable"),
            When(GameServerFound)
                .NotifyServerFound()
                .SaveGameServerInfo()
                .If(condition => condition.Saga.CurrentState != (int)MatchFlowState.Running,
                 callback => callback.TransitionTo(ServerFound)),
            When(GameServerReservationPeriodExpired)
                .TransitionTo(GameServerSearchExpired)
                .NotifyMatchAwaitingExpired(),
            When(CancellationRequested)
                .CancelMatch(reason => reason.Reason));

        During(ServerFound, GameServerSearching,
            When(ReadyAcceptConnections)
                .NotifyMatchBegun()
                .TransitionTo(Running));

        During(Running,
            Ignore(GameServerFound),
            When(CancellationRequested)
                .CancelMatch(reason => reason.Reason),
            When(MatchCompleted)
                .TransitionTo(ProcessingWinnings)
                .ProcessWinnings());

        During(ProcessingWinnings,
            When(WinningsProcessed)
                .Finalize(),
            When(ProcessingWinningsFailed)
                .NotifyProccessingWinningsFailed()
                .TransitionTo(ProcessingWinningsFail));

        WhenEnter(Final, x =>
        {
            // TODO: add match to archive/history service
            return x;
        });

        SetCompletedWhenFinalized();
    }

    public State AwaitingHoldBalances { get; set; }
    public State GameServerSearching { get; set; }
    public State GameServerSearchExpired { get; set; }
    public State Running { get; set; }
    public State ProcessingWinnings { get; set; }
    public State ProcessingWinningsFail { get; set; }
    public State ServerFound { get; set; }

    public Event<IMatchHubReady> MatchHubReady { get; }
    public Event<IBalancesHeld> BalancesHeld { get; }
    public Event<IWinningsProcessed> WinningsProcessed { get; }
    public Event<IGameServerReservationPeriodExpired> GameServerReservationPeriodExpired { get; }
    public Event<IGameServerFound> GameServerFound { get; }
    public Event<IGameServeReadyAcceptConnections> ReadyAcceptConnections { get; }
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
            c.Interval(6, TimeSpan.FromSeconds(10));
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

            ctx.Saga.BetCurrencyId = hub.Bet.CurrencyId;
            ctx.Saga.BetAmount = hub.Bet.Amount;
            ctx.Saga.GameId = hub.GameId;
            ctx.Saga.Players = hub.Rooms.SelectMany(r => r.Members,
                (r, m) => new Player(m.Id, m.Name, r.Index));
            ctx.Saga.CorrelationId = ctx.Message.CorrelationId;
            ctx.Saga.Created = ctx.Message.TimeStamp;
        });
    }

    public static EventActivityBinder<MatchFlow, IBalancesHeld> StartGameServerSearching(
    this EventActivityBinder<MatchFlow, IBalancesHeld> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<ISearchGameServer>(new
        {
            ctx.Message.CorrelationId,
            ctx.Saga.GameId,
            ctx.Saga.Players,
            ctx.Saga.Properties
        }));
    }

    public static EventActivityBinder<MatchFlow, IMatchHubReady> HoldBalances(
    this EventActivityBinder<MatchFlow, IMatchHubReady> binder)
    {
        return binder.SendCommandAsync(Svc.Wallet, ctx => ctx.Init<IHoldBalances>(new
        {
            ctx.Message.CorrelationId,
            Funds = new FundsInfo(ctx.Saga.BetCurrencyId, ctx.Saga.BetAmount),
            UsersIds = ctx.Saga.Players.Select(x => x.Id)
        }));
    }

    public static EventActivityBinder<MatchFlow, IGameServeReadyAcceptConnections> NotifyMatchBegun(
    this EventActivityBinder<MatchFlow, IGameServeReadyAcceptConnections> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyMatchBegun>(new
        {
            ctx.Message.CorrelationId,
            ctx.Saga.Endpoint,
            ctx.Saga.Tickets
        }));
    }

    public static EventActivityBinder<MatchFlow, IGameServerFound> NotifyServerFound(
    this EventActivityBinder<MatchFlow, IGameServerFound> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyServerFound>(new
        {
            ctx.Message.CorrelationId,
        }));
    }

    public static EventActivityBinder<MatchFlow, IGameServerFound> SaveGameServerInfo(
    this EventActivityBinder<MatchFlow, IGameServerFound> binder)
    {
        return binder.Then(x =>
        {
            x.Saga.Endpoint = x.Message.Endpoint.ToString();
            x.Saga.Tickets = x.Message.Tickets;
        });
    }

    public static EventActivityBinder<MatchFlow, TData> CancelMatch<TData>(
    this EventActivityBinder<MatchFlow, TData> binder, Func<TData, string> selectReason)
    where TData : class
    {
        return binder
            .SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyMatchCanceled>(new
            {
                ctx.Saga.CorrelationId,
                Reason = selectReason.Invoke(ctx.Message)
            }))
            .SendCommandAsync(Svc.Wallet, ctx => ctx.Init<IUnHoldBalances>(new
            {
                Funds = new FundsInfo(ctx.Saga.BetCurrencyId, ctx.Saga.BetAmount),
                UsersIds = ctx.Saga.Players.Select(x => x.Id)
            }))
            .Then(x => x.Saga.CanceledReason = selectReason(x.Message))
            .Finalize();
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

    public static EventActivityBinder<MatchFlow, IMatchCompleted> ProcessWinnings(
    this EventActivityBinder<MatchFlow, IMatchCompleted> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<IProcessWinnings>(new
        {
            Funds = new FundsInfo(ctx.Saga.BetCurrencyId, ctx.Saga.BetAmount),
            ctx.Message.Winners,
            ctx.Message.CorrelationId
        }));
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
