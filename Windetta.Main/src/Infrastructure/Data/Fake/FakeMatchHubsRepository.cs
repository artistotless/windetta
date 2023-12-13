using Windetta.Common.Types;
using Windetta.Main.Core.Domain.MatchHubs;
using Windetta.Main.Core.Domain.MatchHubs.Dtos;

namespace Windetta.Main.Infrastructure.Data.Fake;

[AutoInjectExclude]
public class FakeMatchHubsRepository : IMatchHubs
{
    private readonly List<IMatchHub> _hubs = new();

    public Task AddAsync(IMatchHub hub)
    {
        _hubs.Add(hub);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<MatchHubDto>> GetAllAsync()
    {
        return Task.FromResult(_hubs
            .Select(h => h is TournamentMatchHub ? new TournamentMatchHubDto(h) : new MatchHubDto(h))
            .AsEnumerable());
    }

    public Task<IMatchHub> GetAsync(Guid hubId)
    {
        return Task.FromResult(_hubs.First(x => x.Id == hubId));
    }

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
