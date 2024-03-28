using Windetta.Main.Core.Services.LSPM;
public class InMemoryLspms : ILspms
{
    private readonly IList<Lspm> _items;

    public InMemoryLspms()
    {
        _items = new List<Lspm>
        {
            new Lspm()
            {
                Endpoint = new Uri("https://localhost:65275"),
                Id = Guid.NewGuid(),
                Load = 0,
                SupportedGames = [Guid.Parse("accea9d1-7f70-40e2-8a8d-a90d3a79842b")],
            }
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