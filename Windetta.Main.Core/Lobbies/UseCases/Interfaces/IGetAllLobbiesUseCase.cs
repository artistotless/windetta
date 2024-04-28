using Windetta.Main.Core.Lobbies.Dtos;

namespace Windetta.Main.Core.Lobbies.UseCases;

public interface IGetAllLobbiesUseCase : ILobbyUseCase
{
    Task<IEnumerable<LobbyDto>> ExecuteAsync();
}


