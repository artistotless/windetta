using LSPM.Models;
using MassTransit;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Lobbies;

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
}

public enum MatchFlowState : int
{
    AwaitingHoldBalances = 3,
    GameServerSearching = 4,
    Running = 5,
    ProcessingWinnings = 6,
    ProcessingWinningsFail = 7,
    ServerFound = 8,
}

public class MatchFlowStateMachine : MassTransitStateMachine<MatchFlow>
{
    public MatchFlowStateMachine(ILobbies lobbies)
    {
        InstanceState(instance => instance.CurrentState);

        Initially(
            When(LobbyReady)
                .CopyDataToInstance(lobbies)
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
                .NotifyMatchAwaitingExpired()
                .CancelMatch(reason => "GameServers unavailable"),
            When(CancellationRequested)
                .CancelMatch(reason => reason.Reason));

        During(GameServerSearching, Running,
             When(GameServerFound)
                .SaveGameServerInfo()
                .NotifyServerFound()
                .If(condiction => condiction.Saga.CurrentState != (int)MatchFlowState.Running,
                then => then.TransitionTo(ServerFound)));

        During(ServerFound, GameServerSearching,
            When(ReadyAcceptConnections)
                .NotifyReadyToConnect()
                .TransitionTo(Running));

        During(Running,
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
    public State Running { get; set; }
    public State ProcessingWinnings { get; set; }
    public State ProcessingWinningsFail { get; set; }
    public State ServerFound { get; set; }

    public Event<ILobbyReady> LobbyReady { get; }
    public Event<IBalancesHeld> BalancesHeld { get; }
    public Event<IWinningsProcessed> WinningsProcessed { get; }
    public Event<IGameServerFound> GameServerFound { get; }
    public Event<IGameServeReadyAcceptConnections> ReadyAcceptConnections { get; }
    public Event<ICancellationMatchRequested> CancellationRequested { get; }
    public Event<IMatchCompleted> MatchCompleted { get; }

    // Faults
    public Event<Fault<IHoldBalances>> HoldBalancesFailed { get; }
    public Event<Fault<ISearchGameServer>> GameServerReservationFailed { get; }
    public Event<Fault<IProcessWinnings>> ProcessingWinningsFailed { get; }
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
    public static EventActivityBinder<MatchFlow, ILobbyReady> CopyDataToInstance(
        this EventActivityBinder<MatchFlow, ILobbyReady> binder, ILobbies lobbies)
    {
        return binder.Then(async ctx =>
        {
            var lobby = await lobbies.GetAsync(ctx.Message.CorrelationId);

            if (lobby is null)
                throw LobbyException.NotFound;

            ctx.Saga.Properties = lobby.Properties;
            ctx.Saga.BetCurrencyId = lobby.Bet.CurrencyId;
            ctx.Saga.BetAmount = lobby.Bet.Amount;
            ctx.Saga.GameId = lobby.GameId;
            ctx.Saga.Players = lobby.Rooms.SelectMany(r => r.Members,
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

    public static EventActivityBinder<MatchFlow, ILobbyReady> HoldBalances(
    this EventActivityBinder<MatchFlow, ILobbyReady> binder)
    {
        return binder.SendCommandAsync(Svc.Wallet, ctx => ctx.Init<IHoldBalances>(new
        {
            ctx.Message.CorrelationId,
            Funds = new FundsInfo(ctx.Saga.BetCurrencyId, ctx.Saga.BetAmount),
            UsersIds = ctx.Saga.Players.Select(x => x.Id)
        }));
    }

    public static EventActivityBinder<MatchFlow, TData> NotifyReadyToConnect<TData>(
    this EventActivityBinder<MatchFlow, TData> binder)
    where TData : class, CorrelatedBy<Guid>
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyReadyToConnect>(new
        {
            ctx.Message.CorrelationId,
            Players = ctx.Saga.Players.Select(p => p.Id),
        }));
    }

    public static EventActivityBinder<MatchFlow, IGameServerFound> NotifyServerFound(
    this EventActivityBinder<MatchFlow, IGameServerFound> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyServerFound>(new
        {
            ctx.Message.CorrelationId,
            Endpoint = new Uri(ctx.Saga.Endpoint!),
            Tickets = ctx.Saga.Tickets!
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
            .Finalize();
    }

    public static EventActivityBinder<MatchFlow, Fault<IProcessWinnings>> NotifyProccessingWinningsFailed(
    this EventActivityBinder<MatchFlow, Fault<IProcessWinnings>> binder)
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

    public static EventActivityBinder<MatchFlow, Fault<ISearchGameServer>> NotifyMatchAwaitingExpired(
    this EventActivityBinder<MatchFlow, Fault<ISearchGameServer>> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyMatchAwaitingExpired>(new
        {
            ctx.Message.Message.CorrelationId,
        }));
    }
    #endregion
}
