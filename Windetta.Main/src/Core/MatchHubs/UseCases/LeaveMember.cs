using Windetta.Main.Core.Exceptions;
using Windetta.Main.MatchHubs;

namespace Windetta.Main.Core.MatchHubs.UseCases;

public class LeaveMember : ILeaveMemberMatchHubUseCase
{
    private readonly IMatchHubs _hubs;

    public LeaveMember(IMatchHubs hubs)
    {
        _hubs = hubs;
    }

    public async Task ExecuteAsync(Guid userId, Guid hubId)
    {
        var hub = await _hubs.GetAsync(hubId);

        if (hub is null)
            throw MatchHubException.NotFound;

        hub.Remove(userId);
    }
}
