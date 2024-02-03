using Windetta.Common.Types;

namespace Windetta.Main.Core.Lobbies.UseCases;

public interface ILobbyUseCasesFactory : IScopedService
{
    public T Get<T>() where T : ILobbyUseCase;
}