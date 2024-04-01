using Windetta.Main.Core.Exceptions;

namespace Windetta.Main.Core.Lobbies.UseCases;

public class Delete : IDeleteLobbyUseCase
{
    private readonly ILobbies _lobbies;

    public Delete(ILobbies lobbies)
    {
        _lobbies = lobbies;
    }

    public async Task ExecuteAsync(Guid lobbyId)
    {
        var lobby = await _lobbies.GetAsync(lobbyId);

        if (lobby is null)
            throw LobbyException.NotFound;

        lobby.Dispose();
    }
}