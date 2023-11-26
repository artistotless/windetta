using Windetta.Main.MatchHub;

namespace Windetta.MainTests.Mocks;

internal class MatchHubsInMemoryStorage : IMatchHubs
{
    public int Count => _hubs.Count;

    private readonly List<IMatchHub> _hubs;

    public MatchHubsInMemoryStorage()
    {
        _hubs = new();
    }

    public Task AddAsync(IMatchHub hub)
    {
        _hubs.Add(hub);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<IMatchHub>> GetAllAsync()
        => Task.FromResult(_hubs.AsEnumerable());

    public Task<IMatchHub> GetAsync(Guid hubId)
        => Task.FromResult(_hubs.First(x => x.Id == hubId));

    public Task RemoveAsync(IMatchHub hub)
    {
        _hubs.Remove(hub);

        return Task.CompletedTask;
    }
}
