using MassTransit;
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
            .CreateWallet()
            .TransitionTo(AwaitingConfirmation));

        During(AwaitingConfirmation,
            When(EmailConfirmed)
            .TransitionTo(Confirmed),
            Ignore(UserCreated));

        During(Confirmed,
            Ignore(EmailConfirmed),
            Ignore(UserCreated));
    }

    public State AwaitingConfirmation { get; set; }
    public State Confirmed { get; set; }

    public Event<IUserCreated> UserCreated { get; }
    public Event<IUserEmailConfirmed> EmailConfirmed { get; }

}

public static class NewUserFlowStateMachineExtensions
{
    public static EventActivityBinder<NewUserFlow, T> CreateWallet<T>(
       this EventActivityBinder<NewUserFlow, T> binder) where T : class
    {
        return binder.SendAsync(context => context.Init<ICreateUserWallet>(new
        {
            context.Saga.UserId,
        }));
    }

    public static EventActivityBinder<NewUserFlow, T> NotifyEmailConfirmation<T>(
        this EventActivityBinder<NewUserFlow, T> binder) where T : class
    {
        return binder.SendAsync(context => context.Init<INotifyEmailConfirmation>(new
        {
            context.Saga.UserId,
            context.Saga.Email,
        }));
    }

    public static EventActivityBinder<NewUserFlow, IUserCreated> NotifyUserCreated(
        this EventActivityBinder<NewUserFlow, IUserCreated> binder)
    {
        return binder.SendAsync(context => context.Init<INotifyUserCreated>(new
        {
            UserId = context.Message.Id,
            context.Message.Email,
            context.Message.TimeStamp
        }));
    }
}