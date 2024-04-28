using Windetta.Main.Core.Exceptions;

namespace Windetta.Main.Core.Lobbies.UseCases;

public class LeaveMember : ILeaveMemberLobbyUseCase
{
    private readonly ILobbies _lobbies;

    public LeaveMember(ILobbies lobbies)
    {
        _lobbies = lobbies;
    }

    public async Task ExecuteAsync(Guid userId, Guid lobbyId, ushort roomIndex)
    {
        var lobby = await _lobbies.GetAsync(lobbyId);

        if (lobby is null)
            throw LobbyException.NotFound;

        if (lobby.State == LobbyState.Ready)
            throw new LobbyException
                ("cannot_leave_when_lobby_ready");

        lobby.RemoveMember(userId, roomIndex);
    }
}
