using System.Collections.Concurrent;
using Windetta.Common.Types;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;

namespace Windetta.Main.Infrastructure.Data;

[AutoInjectExclude]
public class LobbiesFromMemoryDecorator : ILobbies
{
    private readonly ILobbies _decoratee;
    private readonly ConcurrentDictionary<Guid, ILobby> _inMemoryStorage;

    public LobbiesFromMemoryDecorator(ILobbies decoratee)
    {
        _decoratee = decoratee;
        _inMemoryStorage = new();
    }

    public async Task AddAsync(ILobby lobby)
    {
        _inMemoryStorage.AddOrUpdate(lobby.Id, lobby, (id, h) => lobby);

        await _decoratee.AddAsync(lobby);
    }

    public async Task<IEnumerable<LobbyDto>> GetAllAsync()
    {
        return await _decoratee.GetAllAsync();
    }

    public Task<ILobby?> GetAsync(Guid lobbyId)
    {
        return Task.FromResult(_inMemoryStorage.GetValueOrDefault(lobbyId));
    }

    public async Task RemoveAsync(Guid lobbyId)
    {
        _inMemoryStorage.TryRemove(lobbyId, out _);

        await _decoratee.RemoveAsync(lobbyId);
    }

    public async Task UpdateAsync(ILobby lobby)
    {
        if (_inMemoryStorage.TryGetValue(lobby.Id, out var value))
            _inMemoryStorage.TryUpdate(lobby.Id, lobby, value);

        await _decoratee.UpdateAsync(lobby);
    }
}
