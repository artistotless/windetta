namespace Windetta.Main.Core.Domain.MatchHubs.UseCases;

public interface IJoinMemberMatchHubUseCase : IMatchHubUseCase
{
    Task ExecuteAsync(Guid userId, Guid hubId, Guid roomId);
}