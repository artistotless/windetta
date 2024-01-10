namespace Windetta.Main.Core.Lobbies.UseCases;

public interface IDeleteLobbyUseCase : ILobbyUseCase
{
    Task ExecuteAsync(Guid lobbyId);
}
