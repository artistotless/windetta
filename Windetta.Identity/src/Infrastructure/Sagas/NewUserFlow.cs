using MassTransit;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;

namespace Windetta.Identity.Infrastructure.Sagas;

public class NewUserFlow : SagaStateMachineInstance
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public int CurrentState { get; set; }
    public Guid CorrelationId { get; set; }
}

public enum NewUserFlowState : int
{
    AwaitingConfirmation = 3,
    Confirmed = 4,
}

public class NewUserFlowStateMachine : MassTransitStateMachine<NewUserFlow>
{
    public NewUserFlowStateMachine()
    {
        Event(() => UserCreated, x => x.CorrelateById(ctx => ctx.Message.Id));
        Event(() => EmailConfirmed, x => x.CorrelateById(ctx => ctx.Message.UserId));

        InstanceState(instance => instance.CurrentState);

        Initially(
            When(UserCreated)
            .Then(ctx =>
            {
                ctx.Saga.UserId = ctx.Message.Id;
                ctx.Saga.Email = ctx.Message.Email;
            })
            .NotifyEmailConfirmation()
            .NotifyUserCreated()
            .TransitionTo(AwaitingConfirmation));

        During(AwaitingConfirmation,
            When(EmailConfirmed)
            .Finalize());

        SetCompletedWhenFinalized();
    }

    public State AwaitingConfirmation { get; set; }
    public State Confirmed { get; set; }

    public Event<IUserCreated> UserCreated { get; }
    public Event<IUserEmailConfirmed> EmailConfirmed { get; }
}

public static class NewUserFlowStateMachineExtensions
{
    public static EventActivityBinder<NewUserFlow, IUserCreated> NotifyEmailConfirmation(
        this EventActivityBinder<NewUserFlow, IUserCreated> binder)
    {
        return binder.SendCommandAsync(Svc.Notifications, ctx => ctx.Init<INotifyEmailConfirmation>(new
        {
            UserId = ctx.Message.Id,
            ctx.Message.Email
        }));
    }

    public static EventActivityBinder<NewUserFlow, IUserCreated> NotifyUserCreated(
        this EventActivityBinder<NewUserFlow, IUserCreated> binder)
    {
        return binder.SendCommandAsync(Svc.Notifications, ctx => ctx.Init<INotifyUserCreated>(new
        {
            UserId = ctx.Message.Id,
            ctx.Message.Email,
            ctx.Message.TimeStamp
        }));


    }
}