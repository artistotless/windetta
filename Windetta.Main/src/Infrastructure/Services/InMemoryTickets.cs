using LSPM.Models;
using System.Collections.Concurrent;
using Windetta.Main.Core.Matches;

namespace Windetta.Main.Infrastructure.Services;

public sealed class InMemoryTickets : ITickets
{
    private readonly ConcurrentDictionary<TicketKey, string> _storage;

    public InMemoryTickets()
    {
        _storage = new();
    }

    public Task<string> GetAsync(TicketKey key)
        => Task.FromResult(_storage[key]);

    public Task SetRangeAsync(IEnumerable<Ticket> tickets)
    {
        foreach (var ticket in tickets)
        {
            _storage.AddOrUpdate(new TicketKey(ticket.MatchId, ticket.PlayerId),
                ticket.Value, (key, old) => ticket.Value);
        }

        return Task.CompletedTask;
    }

    public Task UnsetAsync(TicketKey key)
    {
        _storage.TryRemove(key, out _);

        return Task.CompletedTask;
    }
}