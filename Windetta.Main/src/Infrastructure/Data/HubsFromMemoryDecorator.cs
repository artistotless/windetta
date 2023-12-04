using System.Collections.Concurrent;
using Windetta.Common.Types;
using Windetta.Main.Core.MatchHubs.Dtos;
using Windetta.Main.MatchHubs;

namespace Windetta.Main.Infrastructure.Data;

[AutoInjectExclude]
public class HubsFromMemoryDecorator : IMatchHubs
{
    private readonly IMatchHubs _decoratee;
    private readonly ConcurrentDictionary<Guid, IMatchHub> _inMemoryStorage;

    public HubsFromMemoryDecorator(IMatchHubs decoratee)
    {
        _decoratee = decoratee;
        _inMemoryStorage = new();
    }

    public async Task AddAsync(IMatchHub hub)
    {
        _inMemoryStorage.AddOrUpdate(hub.Id, hub, (id, h) => hub);

        await _decoratee.AddAsync(hub);
    }

    public async Task<IEnumerable<MatchHubDto>> GetAllAsync()
    {
        return await _decoratee.GetAllAsync();
    }

    public Task<IMatchHub?> GetAsync(Guid hubId)
    {
        return Task.FromResult(_inMemoryStorage.GetValueOrDefault(hubId));
    }

    public async Task RemoveAsync(Guid hubId)
    {
        _inMemoryStorage.TryRemove(hubId, out _);

        await _decoratee.RemoveAsync(hubId);
    }

    public async Task UpdateAsync(IMatchHub hub)
    {
        if (_inMemoryStorage.TryGetValue(hub.Id, out var value))
            _inMemoryStorage.TryUpdate(hub.Id, hub, value);

        await _decoratee.UpdateAsync(hub);
    }
}
