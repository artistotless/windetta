using Windetta.Main.Core.MatchHubs.Dtos;
using Windetta.Main.MatchHubs;

namespace Windetta.Main.Core.MatchHubs.UseCases;

public class GetAll : IGetAllMatchHubsUseCase
{
    private readonly IMatchHubs _hubs;

    public GetAll(IMatchHubs hubs)
    {
        _hubs = hubs;
    }

    public async Task<IEnumerable<MatchHubDto>> ExecuteAsync()
    {
        return await _hubs.GetAllAsync();
    }
}
