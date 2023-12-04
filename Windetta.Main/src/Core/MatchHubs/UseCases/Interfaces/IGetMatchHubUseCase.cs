using Windetta.Main.MatchHubs;

namespace Windetta.Main.Core.MatchHubs.UseCases;

public interface IGetMatchHubUseCase : IMatchHubUseCase
{
    Task<IMatchHub> ExecuteAsync(Guid hubId);
}