using Windetta.Main.Core.MatchHubs.Dtos;
using Windetta.Main.MatchHubs;

namespace Windetta.MainTests.Mocks;

internal class InMemoryMatchHubsStorage : IMatchHubs
{
    public int Count => _hubs.Count;

    private readonly List<IMatchHub> _hubs;

    public InMemoryMatchHubsStorage()
    {
        _hubs = new();
    }

    public Task AddAsync(IMatchHub hub)
    {
        _hubs.Add(hub);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<MatchHubDto>> GetAllAsync()
        => Task.FromResult(_hubs.Select(h => new MatchHubDto(h)));

    public Task<IMatchHub> GetAsync(Guid hubId)
        => Task.FromResult(_hubs.First(x => x.Id == hubId));

    public Task RemoveAsync(Guid hubId)
    {
        _hubs.RemoveAll(x => x.Id == hubId);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(IMatchHub hub)
    {
        return Task.CompletedTask;
    }
}
