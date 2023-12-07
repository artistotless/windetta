using MongoDB.Driver;
using Windetta.Common.Types;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Core.MatchHubs.Dtos;
using Windetta.Main.Infrastructure.Data.DbContexts;

namespace Windetta.Main.Infrastructure.Data.EFCore;

[AutoInjectExclude]
public class EFCoreMatchHubsRepository : IMatchHubs
{

    private readonly MatchHubsDbContext _context;

    public EFCoreMatchHubsRepository(MatchHubsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(IMatchHub hub)
    {
        _context.MatchHubs.Add(MatchHubDbModel.MapFrom(hub));

        await _context.SaveChangesAsync();
    }

    public Task<IEnumerable<MatchHubDto>> GetAllAsync()
    {
        return Task.FromResult(_context.MatchHubs.Select(MatchHubDbModel.MapTo));
    }

    public Task<IMatchHub?> GetAsync(Guid hubId)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(Guid hubId)
    {
        var hub = _context.MatchHubs.FirstOrDefault(x => x.Id == hubId);

        if (hub is null)
            return;

        _context.MatchHubs.Remove(hub);

        await _context.SaveChangesAsync();
    }

    public Task UpdateAsync(IMatchHub hub)
    {
        return Task.CompletedTask;
    }
}
