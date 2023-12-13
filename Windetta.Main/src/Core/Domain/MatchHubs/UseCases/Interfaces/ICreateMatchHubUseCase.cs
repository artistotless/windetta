namespace Windetta.Main.Core.Domain.MatchHubs.UseCases;

public interface ICreateMatchHubUseCase : IMatchHubUseCase
{
    Task<IMatchHub> ExecuteAsync(MatchHubOptions options);
}
