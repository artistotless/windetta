using Windetta.Common.Testing;
using Windetta.Main.Core.Services.LSPM;

namespace Windetta.Main.Infrastructure.Services;

public sealed class InMemoryLspms : ILspms
{
    private readonly IList<Lspm> _items;

    public InMemoryLspms()
    {
        _items = new List<Lspm>
        {
            new Lspm()
            {
                Endpoint = new Uri("https://localhost:55005"),
                Id = Guid.NewGuid(),
                Load = 0,
                SupportedGames = [ ExampleGuids.GameId],
            },

            new Lspm()
            {
                Endpoint = new Uri("https://gs1-node.feed78.com:55005"),
                Id = Guid.NewGuid(),
                Load = 0,
                SupportedGames = [ ExampleGuids.GameId],
            },
        };
    }

    public Task<IEnumerable<Lspm>> GetAllAsync()
    {
        return Task.FromResult(_items.AsEnumerable());
    }

    public Task<IEnumerable<Lspm>> GetAllAsync(Guid gameId)
    {
        return Task.FromResult(_items
            .Where(l => l.SupportedGames
            .Contains(gameId))
            .AsEnumerable());
    }

    public Task<Lspm?> GetAsync(Guid gameId)
    {
        return Task.FromResult(_items
            .FirstOrDefault(l => l.SupportedGames
            .Contains(gameId)));
    }
}
