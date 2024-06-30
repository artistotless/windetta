using MassTransit;
using Windetta.Contracts.Events;
using Windetta.Contracts.Responses;

namespace Windetta.MainTests.Mocks;

public class AlwaysRespondsSuccessLspmConsumer : IConsumer<IGameServerRequested>
{
    public Task Consume(ConsumeContext<IGameServerRequested> context)
    {
        return context.RespondAsync<GameServerResponse>(new()
        {
            GameServerId = Guid.NewGuid(),
        });
    }
}

public class AlwaysOverloadLspmConsumer : IConsumer<IGameServerRequested>
{
    public Task Consume(ConsumeContext<IGameServerRequested> context)
    {
        return context.RespondAsync<GameServerResponse>(new()
        {
            Error = "Overload",
        });
    }
}
