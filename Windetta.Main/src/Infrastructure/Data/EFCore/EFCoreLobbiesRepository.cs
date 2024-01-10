using MongoDB.Driver;
using Windetta.Common.Types;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;
using Windetta.Main.Infrastructure.Data.DbContexts;

namespace Windetta.Main.Infrastructure.Data.EFCore;

[AutoInjectExclude]
public class EFCoreLobbiesRepository : ILobbies
{

    private readonly LobbiesDbContext _context;

    public EFCoreLobbiesRepository(LobbiesDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ILobby lobby)
    {
        _context.Lobbies.Add(LobbyDbModel.MapFrom(lobby));

        await _context.SaveChangesAsync();
    }

    public Task<IEnumerable<LobbyDto>> GetAllAsync()
    {
        return Task.FromResult(_context.Lobbies.Select(LobbyDbModel.MapTo));
    }

    public Task<ILobby?> GetAsync(Guid lobbyId)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(Guid lobbyId)
    {
        var lobby = _context.Lobbies.FirstOrDefault(x => x.Id == lobbyId);

        if (lobby is null)
            return;

        _context.Lobbies.Remove(lobby);

        await _context.SaveChangesAsync();
    }

    public Task UpdateAsync(ILobby lobby)
    {
        return Task.CompletedTask;
    }
}
