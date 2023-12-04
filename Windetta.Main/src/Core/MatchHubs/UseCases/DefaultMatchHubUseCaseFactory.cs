namespace Windetta.Main.Core.MatchHubs.UseCases;

public sealed class DefaultMatchHubUseCaseFactory : IMatchHubUseCasesFactory
{
    private readonly IEnumerable<IMatchHubUseCase> _matchHubUseCases;

    public DefaultMatchHubUseCaseFactory(IEnumerable<IMatchHubUseCase> matchHubUseCases)
    {
        _matchHubUseCases = matchHubUseCases;
    }

    public T Get<T>() where T : IMatchHubUseCase
    {
        var found = _matchHubUseCases.Where(x => x.GetType().IsAssignableTo(typeof(T))).First();

        return (T)found;
    }
}