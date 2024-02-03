namespace Windetta.Main.Core.Lobbies.UseCases;

public interface IGetLobbyIdByUserIdUseCase : ILobbyUseCase
{
    Task<Guid?> ExecuteAsync(Guid userId);
}