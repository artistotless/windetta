using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.MatchHubs;

namespace Windetta.Main.Core.MatchHubs.UseCases;

public class Delete : IDeleteMatchHubUseCase
{
    private readonly IMatchHubs _hubs;

    public Delete(IMatchHubs hubs)
    {
        _hubs = hubs;
    }

    public async Task ExecuteAsync(Guid hubId)
    {
        var hub = await _hubs.GetAsync(hubId);

        if (hub is null)
            throw MatchHubException.NotFound;

        await _hubs.RemoveAsync(hub.Id);

        hub.Dispose();
    }
}