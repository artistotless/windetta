using LSPM.Models;
using MassTransit;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Contracts.Responses;
using Windetta.Main.Core.Matches;

namespace Windetta.Main.Infrastructure.Sagas;

public class MatchFlow : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public IEnumerable<Player> Players { get; set; }
    public int BetCurrencyId { get; set; }
    public ulong BetAmount { get; set; }
    public IReadOnlyDictionary<string, string>? Properties { get; set; }
    public Guid GameId { get; set; }
    public DateTimeOffset Created { get; set; }
    public int CurrentState { get; set; }
    public Uri GameServerEndpoint { get; set; }
}

public enum MatchFlowState : int
{
    CreatingMatch = 3,
    Running = 4,
    ProcessingWinnings = 5,
    ProcessingWinningsFail = 6,
}

public class MatchFlowStateMachine : MassTransitStateMachine<MatchFlow>
{
    public MatchFlowStateMachine(IOngoingMatches matches)
    {
        InstanceState(instance => instance.CurrentState);

        Initially(
            When(FlowCreated)
                .CopyDataToInstance()
                .CreateMatch()
                .TransitionTo(CreatingMatch));

        DuringAny(When(MatchInfoRequested)
            .RespondAsync(x => x.Init<MatchInfoResponse>(new MatchInfoResponse
            {
                Bet = new FundsInfo(x.Saga.BetCurrencyId, x.Saga.BetAmount),
                Created = x.Saga.Created,
                MatchId = x.Saga.CorrelationId,
                GameId = x.Saga.GameId,
                Players = x.Saga.Players,
                GameServerEndpoint = x.Saga.GameServerEndpoint,
            })));

        During(CreatingMatch,
             When(MatchCreated)
                .SaveOngoingMatch(matches)
                .NotifyReadyToConnect()
                .TransitionTo(Running));

        During(Running,
            When(CancellationRequested)
                .RemoveOngoingMatch(matches)
                .CancelMatch(reason => reason.Reason),
            When(MatchCompleted)
                .RemoveOngoingMatch(matches)
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

    public State CreatingMatch { get; set; }
    public State Running { get; set; }
    public State ProcessingWinnings { get; set; }
    public State ProcessingWinningsFail { get; set; }

    public Event<ICreateMatchFlowRequested> FlowCreated { get; }
    public Event<IMatchCreated> MatchCreated { get; }
    public Event<IWinningsProcessed> WinningsProcessed { get; }
    public Event<ICancellationMatchRequested> CancellationRequested { get; }
    public Event<IMatchCompleted> MatchCompleted { get; }
    public Event<IMatchInfoRequested> MatchInfoRequested { get; }

    // Faults
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
    public static EventActivityBinder<MatchFlow, ICreateMatchFlowRequested> CopyDataToInstance(
   this EventActivityBinder<MatchFlow, ICreateMatchFlowRequested> binder)
    {
        return binder.Then(ctx =>
        {
            ctx.Saga.Properties = ctx.Message.Properties;
            ctx.Saga.BetCurrencyId = ctx.Message.Bet.CurrencyId;
            ctx.Saga.BetAmount = ctx.Message.Bet.Amount;
            ctx.Saga.GameId = ctx.Message.GameId;
            ctx.Saga.Players = ctx.Message.Players;
            ctx.Saga.CorrelationId = ctx.Message.CorrelationId;
            ctx.Saga.Created = DateTime.UtcNow;
            ctx.Saga.GameServerEndpoint = ctx.Message.GameServerEndpoint;
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

    public static EventActivityBinder<MatchFlow, ICreateMatchFlowRequested> CreateMatch(
    this EventActivityBinder<MatchFlow, ICreateMatchFlowRequested> binder)
    {
        return binder.PublishAsync(ctx => ctx.Init<ICreateMatchRequested>(new
        {
            ctx.Message.GameServerId,
            ctx.Saga.Players,
            ctx.Saga.Properties,
            ctx.Message.CorrelationId,
            ctx.Message.LspmIp,
        }));
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

    public static EventActivityBinder<MatchFlow, IMatchCreated> SaveOngoingMatch(
    this EventActivityBinder<MatchFlow, IMatchCreated> binder, IOngoingMatches matches)
    {
        return binder.Then(async ctx =>
        {
            var ongoingMatch = new OngoingMatchPlayersReference(
                ctx.Message.CorrelationId,
                ctx.Saga.Players);

            await matches.AddAsync(ongoingMatch);
        });
    }

    public static EventActivityBinder<MatchFlow, TData> RemoveOngoingMatch<TData>(
        this EventActivityBinder<MatchFlow, TData> binder, IOngoingMatches matches) where TData : class
    {
        return binder.Then(async ctx =>
        {
            var endpoint = ctx.Saga.GameServerEndpoint;

            await matches.RemoveAsync(ctx.Saga.CorrelationId);
        });
    }

    public static EventActivityBinder<MatchFlow, IMatchCreated> NotifyReadyToConnect(
    this EventActivityBinder<MatchFlow, IMatchCreated> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<INotifyReadyToConnect>(new
        {
            ctx.Message.CorrelationId,
            PlayersIds = ctx.Saga.Players.Select(p => p.Id),
        }));
    }
    #endregion
}
