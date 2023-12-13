using MassTransit;
using Windetta.Contracts.Events;
using Windetta.Main.Core.Domain.MatchHubs;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Services.LSPM;

namespace Windetta.Operations.Sagas;

public class MatchFlow : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public IEnumerable<Guid> Members { get; set; }
    public Guid GameId { get; set; }
    public string Endpoint { get; set; }
    public DateTimeOffset Created { get; set; }
    public Bet Bet { get; set; }
    public int CurrentState { get; set; }
    public string? CanceledReason { get; set; }
    public Guid? ExpirationTokenId { get; set; }
}

public class MatchFlowStateMachine : MassTransitStateMachine<MatchFlow>
{
    public MatchFlowStateMachine(IMatchHubs hubs, ILspms lspms)
    {
        Event(() => MatchHubReady, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => GameServerPrepared, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => CancellationRequested, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => Completed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

        Schedule(() => ExpirationSchedule,
            x => x.ExpirationTokenId,
            x => x.Delay = TimeSpan.FromMinutes(20));

        InstanceState(instance => instance.CurrentState);

        Initially(
            When(MatchHubReady)
            .CopyDataToInstance(hubs)
            .TransitionTo(ConnectiongToLspm));
    }

    public State ConnectiongToLspm { get; set; }
    public State ConnectiongLspmFail { get; set; }
    public State AwaitingGameServerInfo { get; set; }
    public State Active { get; set; }
    public State Canceled { get; set; }
    public State Expired { get; set; }

    public Event<IMatchHubReady> MatchHubReady { get; }
    public Event<IReservedGameServerPrepared> GameServerPrepared { get; }
    public Event<ICancellationMatchRequested> CancellationRequested { get; }
    public Event<IMatchCompleted> Completed { get; }

    public Schedule<MatchFlow, IGameServerReservationPeriodExpired> ExpirationSchedule { get; }
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
            ctx.Saga.Members = hub.Rooms.SelectMany(r => r.Members, (r, m) => m.Id);
            ctx.Saga.CorrelationId = ctx.Message.CorrelationId;
            ctx.Saga.Created = ctx.Message.TimeStamp;
        });
    }

    public static EventActivityBinder<MatchFlow, IMatchHubReady> ConnectToLspm(
       this EventActivityBinder<MatchFlow, IMatchHubReady> binder, IMatchHubs hubs)
    {

        return binder.Then(async ctx =>
        {
            var hub = await hubs.GetAsync(ctx.Message.CorrelationId);

            if (hub is null)
                throw MatchHubException.NotFound;

            ctx.Saga.Bet = hub.Bet;
            ctx.Saga.GameId = hub.GameId;
            ctx.Saga.Members = hub.Rooms.SelectMany(r => r.Members, (r, m) => m.Id);
            ctx.Saga.CorrelationId = ctx.Message.CorrelationId;
            ctx.Saga.Created = ctx.Message.TimeStamp;
        });
    }
}
#endregion