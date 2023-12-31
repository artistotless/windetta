using LSPM.Models;
using MassTransit;
using Windetta.Contracts.Events;

namespace Windetta.MainTests.Mocks;

public class AlwaysRespondsSuccessLspmConsumer : IConsumer<IGameServerRequested>
{
    public Task Consume(ConsumeContext<IGameServerRequested> context)
    {
        return context.RespondAsync<RequestingGameServerResult>(new()
        {
            Success = true,
            Details = new ConnectionToServerDetails()
            {
                Endpoint = new UriBuilder("udp", "127.0.0.1", 9999).Uri,
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

public class AlwaysOverloadLspmConsumer : IConsumer<IGameServerRequested>
{
    public Task Consume(ConsumeContext<IGameServerRequested> context)
    {
        return context.RespondAsync<RequestingGameServerResult>(new()
        {
            Success = false,
            Details = null,
            Error = null,
        });
    }
}
