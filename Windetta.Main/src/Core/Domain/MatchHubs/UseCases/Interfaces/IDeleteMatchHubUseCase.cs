namespace Windetta.Main.Core.Domain.MatchHubs.UseCases;

public interface IDeleteMatchHubUseCase : IMatchHubUseCase
{
    Task ExecuteAsync(Guid hubId);
}
