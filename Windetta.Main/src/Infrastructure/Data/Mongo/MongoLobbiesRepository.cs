using MongoDB.Driver;
using Windetta.Common.Types;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;
using Windetta.Main.Infrastructure.Services;

namespace Windetta.Main.Infrastructure.Data.Mongo;

[AutoInjectExclude]
public class MongoLobbiesRepository : ILobbies
{
    private readonly MongoDbContext _context;
    private readonly IInstanceIdProvider _idProvider;

    public MongoLobbiesRepository(MongoDbContext context, IInstanceIdProvider idProvider)
    {
        _context = context;
        _idProvider = idProvider;
    }

    public async Task AddAsync(ILobby lobby)
    {
        var mapped = LobbyDbModel.MapFrom(lobby);

        mapped.InstanceId = _idProvider.GetId();

        await _context.LobbiesCollections
            .InsertOneAsync(mapped);
    }

    public async Task<IEnumerable<LobbyDto>> GetAllAsync()
    {
        var results = await _context.LobbiesCollections.FindAsync(_ => true);

        return results.ToEnumerable().Select(LobbyDbModel.MapTo);
    }

    public Task<ILobby?> GetAsync(Guid lobbyId)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(Guid lobbyId)
    {
        await _context.LobbiesCollections
            .DeleteOneAsync(h => h.Id == lobbyId && h.InstanceId == _idProvider.GetId());
    }

    public async Task UpdateAsync(ILobby lobby)
    {
        var updated = LobbyDbModel.MapFrom(lobby);
        updated.InstanceId = _idProvider.GetId();

        var result = await _context.LobbiesCollections.ReplaceOneAsync(
                filter: h => h.Id == lobby.Id && h.InstanceId == _idProvider.GetId(),
                replacement: updated,
                options: new ReplaceOptions() { IsUpsert = true });
    }
}
