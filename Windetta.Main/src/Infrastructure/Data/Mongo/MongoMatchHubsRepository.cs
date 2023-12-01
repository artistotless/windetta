using MongoDB.Driver;
using Windetta.Main.Core.MatchHub.Dtos;
using Windetta.Main.Infrastructure.Services;
using Windetta.Main.MatchHub;

namespace Windetta.Main.Infrastructure.Data.Mongo;

public class MongoMatchHubsRepository : IMatchHubs
{
    private readonly MongoDbContext _context;
    private readonly IInstanceIdProvider _idProvider;

    public MongoMatchHubsRepository(MongoDbContext context, IInstanceIdProvider idProvider)
    {
        _context = context;
        _idProvider = idProvider;
    }

    public async Task AddAsync(IMatchHub hub)
    {
        var mapped = MatchHubDbModel.MapFrom(hub);

        mapped.InstanceId = _idProvider.GetId();

        await _context.MatchHubsCollections
            .InsertOneAsync(mapped);
    }

    public async Task<IEnumerable<MatchHubDto>> GetAllAsync()
    {
        var results = await _context.MatchHubsCollections.FindAsync(_ => true);

        return results.ToEnumerable().Select(MatchHubDbModel.MapTo);
    }

    public Task<IMatchHub?> GetAsync(Guid hubId)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(Guid hubId)
    {
        await _context.MatchHubsCollections
            .DeleteOneAsync(h => h.Id == hubId && h.InstanceId == _idProvider.GetId());
    }

    public async Task UpdateAsync(IMatchHub hub)
    {
        await _context.MatchHubsCollections.ReplaceOneAsync(
              filter: h => h.Id == hub.Id && h.InstanceId == _idProvider.GetId(),
              replacement: MatchHubDbModel.MapFrom(hub),
              options: new ReplaceOptions() { IsUpsert = true });
    }
}
