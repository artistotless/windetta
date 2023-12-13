namespace Windetta.Main.Core.Domain.MatchHubs.UseCases;

public interface ILeaveMemberMatchHubUseCase : IMatchHubUseCase
{
    Task ExecuteAsync(Guid userId, Guid hubId);
}