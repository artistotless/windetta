namespace Windetta.Main.Core.MatchHubs.UseCases;

public interface IJoinMemberMatchHubUseCase : IMatchHubUseCase
{
    Task ExecuteAsync(Guid userId, Guid hubId, ushort roomIndex);
}