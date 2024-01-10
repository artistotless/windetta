using Windetta.Common.Types;
using Windetta.Main.Core.Lobbies.Dtos;

namespace Windetta.Main.Core.Lobbies;

public interface ILobbies : ISingletonService
{
    public Task AddAsync(ILobby lobby);
    public Task RemoveAsync(Guid lobbyId);
    public Task<IEnumerable<LobbyDto>> GetAllAsync();
    public Task<ILobby?> GetAsync(Guid lobbyId);
    public Task UpdateAsync(ILobby lobby);
}
