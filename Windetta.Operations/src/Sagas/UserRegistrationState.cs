using MassTransit;
using Windetta.Contracts.Events;

namespace Windetta.Operations.Sagas;

public class UserRegistrationState : SagaStateMachineInstance
{
    public Guid UserId { get; set; }
    public Guid WalletId { get; set; }
    public int CurrentState { get; set; }
    public Guid CorrelationId { get; set; }
}

public class UserRegistrationStateMachine : MassTransitStateMachine<UserRegistrationState>
{
    public UserRegistrationStateMachine()
    {
        Event(() => UserCreated, x => x.CorrelateById(ctx => ctx.Message.Id));

        InstanceState(instance => instance.CurrentState);

        Initially(
            When(UserCreated)
            .Then(ctx => { ctx.Saga.UserId = ctx.Message.Id; })
            //.SendAsync(
            .TransitionTo(Registered));

        During(Registered,
            Ignore(UserCreated));
    }

    public State Registered { get; }
    public State WalletAttached { get; }

    public Event<IUserCreated> UserCreated { get; }
}
