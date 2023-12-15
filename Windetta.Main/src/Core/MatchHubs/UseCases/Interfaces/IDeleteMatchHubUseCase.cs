namespace Windetta.Main.Core.MatchHubs.UseCases;

public interface IDeleteMatchHubUseCase : IMatchHubUseCase
{
    Task ExecuteAsync(Guid hubId);
}
