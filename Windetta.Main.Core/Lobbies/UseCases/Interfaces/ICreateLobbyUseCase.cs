namespace Windetta.Main.Core.Lobbies.UseCases;

public interface ICreateLobbyUseCase : ILobbyUseCase
{
    Task<ILobby> ExecuteAsync(LobbyOptions options);
}
