using Windetta.Main.MatchHubs;

namespace Windetta.Main.Core.MatchHubs.UseCases;

public interface ICreateMatchHubUseCase : IMatchHubUseCase
{
    Task<IMatchHub> ExecuteAsync(MatchHubOptions options);
}
