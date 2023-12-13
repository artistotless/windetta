using Windetta.Main.Core.Domain.MatchHubs.Dtos;

namespace Windetta.Main.Core.Domain.MatchHubs.UseCases;

public interface IGetAllMatchHubsUseCase : IMatchHubUseCase
{
    Task<IEnumerable<MatchHubDto>> ExecuteAsync();
}


