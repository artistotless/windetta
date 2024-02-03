using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;


namespace Windetta.Main.Infrastructure.Data.Fake;

//[AutoInjectExclude]
public class FakeLobbiesRepository : ILobbies
{
    private readonly List<ILobby> _lobbies = new();

    public Task AddAsync(ILobby lobby)
    {
        _lobbies.Add(lobby);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<LobbyDto>> GetAllAsync()
    {
        return Task.FromResult(_lobbies
            .Select(h => new LobbyDto(h))
            .AsEnumerable());
    }

    public Task<ILobby?> GetAsync(Guid lobbyId)
    {
        return Task.FromResult(_lobbies
            .FirstOrDefault(x => x.Id == lobbyId));
    }

    public Task RemoveAsync(Guid lobbyId)
    {
        _lobbies.RemoveAll(x => x.Id == lobbyId);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(ILobby lobby)
    {
        return Task.CompletedTask;
    }
}
