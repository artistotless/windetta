namespace Windetta.Main.Core.MatchHubs.UseCases;

public interface ILeaveMemberMatchHubUseCase : IMatchHubUseCase
{
    Task ExecuteAsync(Guid userId, Guid hubId);
}