using Windetta.Main.Core.Exceptions;
using Windetta.Main.MatchHubs;

namespace Windetta.Main.Core.MatchHubs.UseCases;

public class Get : IGetMatchHubUseCase
{
    private readonly IMatchHubs _hubs;

    public Get(IMatchHubs hubs)
    {
        _hubs = hubs;
    }

    public async Task<IMatchHub> ExecuteAsync(Guid hubId)
    {
        var hub = await _hubs.GetAsync(hubId);

        if (hub is null)
            throw MatchHubException.NotFound;

        return hub;
    }
}