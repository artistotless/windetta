using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;

namespace Windetta.MainTests.Mocks;

public class InMemoryLobbiesStorage : ILobbies
{
    public int Count => _lobbies.Count;

    private readonly List<ILobby> _lobbies;

    public InMemoryLobbiesStorage()
    {
        _lobbies = new();
    }

    public Task AddAsync(ILobby lobby)
    {
        _lobbies.Add(lobby);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<LobbyDto>> GetAllAsync()
        => Task.FromResult(_lobbies.Select(h => new LobbyDto(h)));

    public Task<ILobby?> GetAsync(Guid lobbyId)
        => Task.FromResult(_lobbies.FirstOrDefault(x => x.Id == lobbyId));

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
