using Windetta.Main.Core.Domain.MatchHubs.Dtos;

namespace Windetta.MainTests.Mocks;

public class InMemoryMatchHubsStorage : IMatchHubs
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

    public Task<IMatchHub?> GetAsync(Guid hubId)
        => Task.FromResult(_hubs.FirstOrDefault(x => x.Id == hubId));

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
