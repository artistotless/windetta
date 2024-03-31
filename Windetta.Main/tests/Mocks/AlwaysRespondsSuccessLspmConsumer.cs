using LSPM.Models;
using MassTransit;
using Windetta.Contracts.Events;

namespace Windetta.MainTests.Mocks;

public class AlwaysRespondsSuccessLspmConsumer : IConsumer<IGameServerRequested>
{
    public Task Consume(ConsumeContext<IGameServerRequested> context)
    {
        return context.RespondAsync<GameServerResult>(new()
        {
            GameServerId = Guid.NewGuid(),
        });
    }
}

public class AlwaysOverloadLspmConsumer : IConsumer<IGameServerRequested>
{
    public Task Consume(ConsumeContext<IGameServerRequested> context)
    {
        return context.RespondAsync<GameServerResult>(new()
        {
            Error = "Overload",
        });
    }
}
