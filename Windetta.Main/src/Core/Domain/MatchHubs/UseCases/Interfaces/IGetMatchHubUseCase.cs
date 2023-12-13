namespace Windetta.Main.Core.Domain.MatchHubs.UseCases;

public interface IGetMatchHubUseCase : IMatchHubUseCase
{
    Task<IMatchHub> ExecuteAsync(Guid hubId);
}