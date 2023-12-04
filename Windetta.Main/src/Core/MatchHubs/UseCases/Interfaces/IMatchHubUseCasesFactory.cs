using Windetta.Common.Types;

namespace Windetta.Main.Core.MatchHubs.UseCases;

public interface IMatchHubUseCasesFactory : IScopedService
{
    public T Get<T>() where T : IMatchHubUseCase;
}