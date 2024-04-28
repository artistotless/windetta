namespace Windetta.Main.Core.Lobbies.UseCases;

public interface IGetLobbyUseCase : ILobbyUseCase
{
    Task<ILobby> ExecuteAsync(Guid lobbyId);
}