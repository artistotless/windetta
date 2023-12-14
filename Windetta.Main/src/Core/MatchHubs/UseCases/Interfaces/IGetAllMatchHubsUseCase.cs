using Windetta.Main.Core.MatchHubs.Dtos;

namespace Windetta.Main.Core.MatchHubs.UseCases;

public interface IGetAllMatchHubsUseCase : IMatchHubUseCase
{
    Task<IEnumerable<MatchHubDto>> ExecuteAsync();
}


