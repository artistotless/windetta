using Windetta.Common.Handlers;
using Windetta.Common.RabbitMQ;
using Windetta.Wallet.Messages.Events;

namespace Windetta.Wallet.Handlers.Identity;

public class UserCreatedEventHandler : IEventHandler<UserCreated>
{
    public Task HandleAsync(UserCreated @event, ICorrelationContext context)
    {
        return Task.CompletedTask;
    }
}