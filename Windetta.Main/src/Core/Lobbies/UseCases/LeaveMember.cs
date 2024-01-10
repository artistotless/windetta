using Windetta.Main.Core.Exceptions;

namespace Windetta.Main.Core.Lobbies.UseCases;

public class LeaveMember : ILeaveMemberLobbyUseCase
{
    private readonly ILobbies _lobbies;

    public LeaveMember(ILobbies lobbies)
    {
        _lobbies = lobbies;
    }

    public async Task ExecuteAsync(Guid userId, Guid lobbyId)
    {
        var lobby = await _lobbies.GetAsync(lobbyId);

        if (lobby is null)
            throw LobbyException.NotFound;

        lobby.Remove(userId);
    }
}
