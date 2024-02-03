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
        var result = await client.GetResponse<RequestingGameServerResult>(new
        {
            context.Message.GameId,
            context.Message.CorrelationId,
            context.Message.Players,
            context.Message.Properties,
        });

        if (result.Message.Success)
            await context.Publish<IGameServerFound>(new
            {
                context.Message.CorrelationId,
                result.Message.Details!.Endpoint,
                result.Message.Details!.Tickets,
            });
        else
            throw new Exception();
    }
}
