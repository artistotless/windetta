namespace Windetta.Main.Core.Lobbies.UseCases;

public interface ILeaveMemberLobbyUseCase : ILobbyUseCase
{
    Task ExecuteAsync(Guid userId, Guid lobbyId, ushort roomIndex);
}