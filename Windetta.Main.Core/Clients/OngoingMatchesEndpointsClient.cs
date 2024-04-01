using Windetta.Main.Core.Matches;

namespace Windetta.Main.Clients;

public sealed class OngoingMatchesEndpointsClient : IOngoingMatchesEndpointsClient
{
    private readonly IOngoingMatches _matches;

    public OngoingMatchesEndpointsClient(IOngoingMatches matches)
    {
        _matches = matches;
    }

    public Task<IEnumerable<Guid>> GetAllAsync()
        => _matches.GetAllAsync();

    public Task<OngoingMatch> GetAsync(Guid playerId)
        => _matches.GetAsync(playerId);
}