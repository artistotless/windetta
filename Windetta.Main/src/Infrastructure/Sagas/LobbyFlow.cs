﻿using LSPM.Models;
using MassTransit;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Lobbies;

namespace Windetta.Main.Infrastructure.Sagas;

public class LobbyFlow : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public IEnumerable<Player> Players { get; set; }
    public int BetCurrencyId { get; set; }
    public ulong BetAmount { get; set; }
    public IReadOnlyDictionary<string, string>? Properties { get; set; }
    public Guid LobbyId { get; set; }
    public Guid GameId { get; set; }
    public DateTimeOffset Created { get; set; }
    public int CurrentState { get; set; }
}

public enum LobbyFlowState : int
{
    AwaitingHoldBalances = 3,
    GameServerSearching = 4,
    ServerFound = 5,
}

public class LobbyFlowStateMachine : MassTransitStateMachine<LobbyFlow>
{
    public LobbyFlowStateMachine(ILobbies lobbies, LobbiesInteractor interactor)
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
                .UnholdBalances()
                .NotifyCancelMatch(reason => "One of the players has insufficient balance"));

        During(GameServerSearching,
            When(GameServerReservationFailed)
                .NotifyMatchAwaitingExpired()
                .NotifyCancelMatch(reason => "GameServers unavailable"),
             When(GameServerFound)
                .NotifyServerFound()
                .CreateMatchFlow()
                .DeleteLobby(interactor)
                .TransitionTo(ServerFound)); ;

        SetCompletedWhenFinalized();
    }

    public State AwaitingHoldBalances { get; set; }
    public State GameServerSearching { get; set; }
    public State ServerFound { get; set; }

    public Event<ILobbyReady> LobbyReady { get; }
    public Event<IBalancesHeld> BalancesHeld { get; }
    public Event<IGameServerFound> GameServerFound { get; }

    // Faults
    public Event<Fault<IHoldBalances>> HoldBalancesFailed { get; }
    public Event<Fault<ISearchGameServer>> GameServerReservationFailed { get; }
}

public class LobbyFlowDefinition : SagaDefinition<LobbyFlow>
{
    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<LobbyFlow> sagaConfigurator,
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
public static class LobbyFlowStateMachineExtensions
{
    public static EventActivityBinder<LobbyFlow, ILobbyReady> CopyDataToInstance(
       this EventActivityBinder<LobbyFlow, ILobbyReady> binder, ILobbies lobbies)
    {
        return binder.Then(async ctx =>
        {
            var lobby = await lobbies.GetAsync(ctx.Message.LobbyId);

            if (lobby is null)
                throw LobbyException.NotFound;

            ctx.Saga.CorrelationId = ctx.Message.CorrelationId;
            ctx.Saga.LobbyId = ctx.Message.LobbyId;
            ctx.Saga.Created = ctx.Message.TimeStamp;
            ctx.Saga.GameId = lobby.GameId;
            ctx.Saga.Properties = lobby.Properties;
            ctx.Saga.BetCurrencyId = lobby.Bet.CurrencyId;
            ctx.Saga.BetAmount = lobby.Bet.Amount;
            ctx.Saga.Players = lobby.Rooms.SelectMany(r => r.Members,
                (r, m) => new Player(m.Id, m.Name, r.Index));
        });
    }

    public static EventActivityBinder<LobbyFlow, IGameServerFound> CreateMatchFlow(
    this EventActivityBinder<LobbyFlow, IGameServerFound> binder)
    {
        return binder.PublishAsync(ctx => ctx.Init<ICreateMatchFlowRequested>(new
        {
            CorrelationId = Guid.NewGuid(),
            ctx.Message.GameServerId,
            ctx.Message.GameServerEndpoint,
            ctx.Message.LspmIp,
            Bet = new FundsInfo(ctx.Saga.BetCurrencyId, ctx.Saga.BetAmount),
            ctx.Saga.GameId,
            ctx.Saga.Players,
            ctx.Saga.Properties
        }));
    }

    public static EventActivityBinder<LobbyFlow, IGameServerFound> DeleteLobby(
    this EventActivityBinder<LobbyFlow, IGameServerFound> binder, LobbiesInteractor interactor)
    {
        return binder.ThenAsync(ctx => interactor.DeleteAsync(ctx.Saga.LobbyId));
    }

    public static EventActivityBinder<LobbyFlow, ILobbyReady> HoldBalances(
    this EventActivityBinder<LobbyFlow, ILobbyReady> binder)
    {
        return binder.SendCommandAsync(Svc.Wallet, ctx => ctx.Init<IHoldBalances>(new
        {
            ctx.Message.CorrelationId,
            Funds = new FundsInfo(ctx.Saga.BetCurrencyId, ctx.Saga.BetAmount),
            UsersIds = ctx.Saga.Players.Select(x => x.Id)
        }));
    }

    public static EventActivityBinder<LobbyFlow, IBalancesHeld> StartGameServerSearching(
    this EventActivityBinder<LobbyFlow, IBalancesHeld> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<ISearchGameServer>(new
        {
            ctx.Message.CorrelationId,
            ctx.Saga.GameId,
        }));
    }

    public static EventActivityBinder<LobbyFlow, IGameServerFound> NotifyServerFound(
    this EventActivityBinder<LobbyFlow, IGameServerFound> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyServerFound>(new
        {
            ctx.Saga.LobbyId,
            ctx.Message.CorrelationId,
        }));
    }

    public static EventActivityBinder<LobbyFlow, TData> NotifyCancelMatch<TData>(
    this EventActivityBinder<LobbyFlow, TData> binder, Func<TData, string> selectReason) where TData : class
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyMatchCanceled>(new
        {
            ctx.Saga.LobbyId,
            ctx.Saga.CorrelationId,
            Reason = selectReason.Invoke(ctx.Message)
        }));
    }

    public static EventActivityBinder<LobbyFlow, TData> UnholdBalances<TData>(
    this EventActivityBinder<LobbyFlow, TData> binder) where TData : class
    {
        return binder.SendCommandAsync(Svc.Wallet, ctx => ctx.Init<IUnHoldBalances>(new
        {
            Funds = new FundsInfo(ctx.Saga.BetCurrencyId, ctx.Saga.BetAmount),
            UsersIds = ctx.Saga.Players.Select(x => x.Id)
        }));
    }

    public static EventActivityBinder<LobbyFlow, Fault<ISearchGameServer>> NotifyMatchAwaitingExpired(
    this EventActivityBinder<LobbyFlow, Fault<ISearchGameServer>> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyMatchAwaitingExpired>(new
        {
            ctx.Saga.LobbyId,
            ctx.Message.Message.CorrelationId,
        }));
    }
}
#endregion