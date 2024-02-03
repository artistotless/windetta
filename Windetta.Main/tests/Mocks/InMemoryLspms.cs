using Windetta.Main.Core.Services.LSPM;

namespace Windetta.MainTests.Mocks;

public class InMemoryLspms : ILspms
{
    private readonly IList<Lspm> _items;

    public InMemoryLspms(List<Lspm>? initial = null)
    {
        _items = initial ?? new List<Lspm>();
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