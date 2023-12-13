namespace Windetta.Main.Core.Domain.MatchHubs.UseCases;

public interface IGetMatchHubIdByUserIdUseCase : IMatchHubUseCase
{
    Task<Guid?> ExecuteAsync(Guid userId);
}