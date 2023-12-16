using MassTransit;
using Windetta.Main.Core.Matches;
using Windetta.Main.Core.Services.LSPM;

namespace Windetta.MainTests.Mocks;

public class AlwaysRespondsSuccessLspmConsumer : IConsumer<GameServerRequested>
{
    public Task Consume(ConsumeContext<GameServerRequested> context)
    {
        return context.RespondAsync<RequestingGameServerResult>(new()
        {
            Success = true,
            Info = new GameServerInfo()
            {
                Endpoint = "udp:127.0.0.1",
                Tickets = CreateTickets(context.Message.Players)
            },
            Error = null,
        });
    }

    private Dictionary<Guid, string> CreateTickets(IEnumerable<Player> players)
    {
        var dict = new Dictionary<Guid, string>();

        foreach (var item in players)
        {
            dict.Add(item.Id, $"Ticket#{item.Id}");
        }

        return dict;
    }
}

public class AlwaysOverloadLspmConsumer : IConsumer<GameServerRequested>
{
    public Task Consume(ConsumeContext<GameServerRequested> context)
    {
        return context.RespondAsync<RequestingGameServerResult>(new()
        {
            Success = false,
            Info = null,
            Error = null,
        });
    }
}
