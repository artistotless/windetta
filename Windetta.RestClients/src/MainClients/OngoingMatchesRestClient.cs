using Windetta.Main.Clients;
using Windetta.Main.Core.Matches;

namespace Windetta.RestClients.MainClients;

public sealed class OngoingMatchesRestClient : IOngoingMatchesEndpointsClient
{
    public Task<IEnumerable<Guid>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<OngoingMatch> GetAsync(Guid playerId)
    {
        throw new NotImplementedException();
    }
}