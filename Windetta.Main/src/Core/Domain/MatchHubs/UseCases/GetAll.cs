using Windetta.Main.Core.Domain.MatchHubs.Dtos;

namespace Windetta.Main.Core.Domain.MatchHubs.UseCases;

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
