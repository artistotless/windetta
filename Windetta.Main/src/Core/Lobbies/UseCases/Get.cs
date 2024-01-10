using Windetta.Main.Core.Exceptions;

namespace Windetta.Main.Core.Lobbies.UseCases;

public class Get : IGetLobbyUseCase
{
    private readonly ILobbies _lobbies;

    public Get(ILobbies lobbies)
    {
        _lobbies = lobbies;
    }

    public async Task<ILobby> ExecuteAsync(Guid lobbyId)
    {
        var lobby = await _lobbies.GetAsync(lobbyId);

        if (lobby is null)
            throw LobbyException.NotFound;

        return lobby;
    }
}