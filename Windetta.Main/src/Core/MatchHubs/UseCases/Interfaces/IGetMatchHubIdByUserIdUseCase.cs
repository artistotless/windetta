namespace Windetta.Main.Core.MatchHubs.UseCases;

public interface IGetMatchHubIdByUserIdUseCase : IMatchHubUseCase
{
    Task<Guid?> ExecuteAsync(Guid userId);
}