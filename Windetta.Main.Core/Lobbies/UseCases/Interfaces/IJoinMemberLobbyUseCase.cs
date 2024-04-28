namespace Windetta.Main.Core.Lobbies.UseCases;

public interface IJoinMemberLobbyUseCase : ILobbyUseCase
{
    Task ExecuteAsync(Guid userId, Guid lobbyId, ushort roomIndex);
}