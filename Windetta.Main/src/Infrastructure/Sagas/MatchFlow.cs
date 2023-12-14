using MassTransit;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Matches;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Core.Services.LSPM;

namespace Windetta.Operations.Sagas;

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

public class MatchFlowStateMachine : MassTransitStateMachine<MatchFlow>
{
    public MatchFlowStateMachine(IMatchHubs hubs)
    {
        Event(() => MatchHubReady, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => GameServerPrepared, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => CancellationRequested, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => MatchCompleted, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => GameServerReservationPeriodExpired, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

        InstanceState(instance => instance.CurrentState);

        Initially(
            When(MatchHubReady)
                    .CopyDataToInstance(hubs)
                    .StartGameServerSearching()
                    .TransitionTo(GameServerSearch));

        During(GameServerSearch,
            Ignore(MatchHubReady),

            When(GameServerPrepared)
                .NotifyMatchBegun()
                .TransitionTo(Running),
            When(GameServerReservationPeriodExpired)
                .TransitionTo(GameServerSearchExpired)
                .NotifyMatchAwaitingExpired(),
            When(CancellationRequested)
                .TransitionTo(Canceled));

        During(Running,
            When(CancellationRequested)
                .TransitionTo(Canceled)
                .Then(ctx => { }),
            When(MatchCompleted)
                .TransitionTo(ProcessingWinnings)
                .Then(ctx => { }));
    }

    public State GameServerSearch { get; set; }
    public State GameServerSearchExpired { get; set; }
    public State Running { get; set; }
    public State ProcessingWinnings { get; set; }
    public State Canceled { get; set; }

    public Event<IMatchHubReady> MatchHubReady { get; }
    public Event<IGameServerReservationPeriodExpired> GameServerReservationPeriodExpired { get; }
    public Event<IGameServerPrepared> GameServerPrepared { get; }
    public Event<ICancellationMatchRequested> CancellationRequested { get; }
    public Event<IMatchCompleted> MatchCompleted { get; }
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
            c.Ignore<LspmException>(e => e.ErrorCode == Errors.Main.LspmNotFound);
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

    public static EventActivityBinder<MatchFlow, IMatchHubReady> StartGameServerSearching(
    this EventActivityBinder<MatchFlow, IMatchHubReady> binder)
    {
        return binder.SendCommandAsync(Svc.Main, ctx => ctx.Init<StartSearchingGameServer>(new
        {
            ctx.Message.CorrelationId,
            ctx.Saga.GameId,
            ctx.Saga.Players
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
