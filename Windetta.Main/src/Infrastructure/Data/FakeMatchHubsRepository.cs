using Windetta.Main.MatchHub;

namespace Windetta.Main.Infrastructure.Data;

public class FakeMatchHubsRepository : IMatchHubs
{
    private readonly List<IMatchHub> _hubs = new();

    public Task AddAsync(IMatchHub hub)
    {
        _hubs.Add(hub);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<IMatchHub>> GetAllAsync()
    {
        return Task.FromResult(_hubs.AsEnumerable());
    }

    public Task<IMatchHub> GetAsync(Guid hubId)
    {
        return Task.FromResult(_hubs.First(x => x.Id == hubId));
    }

    public Task RemoveAsync(IMatchHub hub)
    {
        _hubs.Remove(hub);

        return Task.CompletedTask;
    }
}
