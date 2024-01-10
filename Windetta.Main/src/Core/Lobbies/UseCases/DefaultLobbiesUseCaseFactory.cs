namespace Windetta.Main.Core.Lobbies.UseCases;

public sealed class DefaultLobbiesUseCaseFactory : ILobbyUseCasesFactory
{
    private readonly IEnumerable<ILobbyUseCase> _lobbiesUseCases;

    public DefaultLobbiesUseCaseFactory(IEnumerable<ILobbyUseCase> lobbyUseCases)
    {
        _lobbiesUseCases = lobbyUseCases;
    }

    public T Get<T>() where T : ILobbyUseCase
    {
        var found = _lobbiesUseCases
            .Where(x => x.GetType()
            .IsAssignableTo(typeof(T))).First();

        return (T)found;
    }
}