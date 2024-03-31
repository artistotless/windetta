using LSPM.Models;
using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;

namespace Windetta.MainTests.Mocks;

public class TestSearchGameServerConsumer : IConsumer<ISearchGameServer>
{
    private readonly IRequestClient<IGameServerRequested> client;

    public TestSearchGameServerConsumer(IRequestClient<IGameServerRequested> client)
    {
        this.client = client;
    }

    public async Task Consume(ConsumeContext<ISearchGameServer> context)
    {
        var result = await client.GetResponse<GameServerResult>(new
        {
            context.Message.GameId,
            context.Message.CorrelationId,
        });

        if (result.Message.Success)
            await context.Publish<IGameServerFound>(new
            {
                context.Message.CorrelationId,
                result.Message.GameServerId,
                LspmIp = "127.0.0.1"
            });
        else
            throw new Exception();
    }
}
