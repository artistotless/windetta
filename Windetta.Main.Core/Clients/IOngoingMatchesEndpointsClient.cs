using Windetta.Common.Types;
using Windetta.Main.Core.Matches;

namespace Windetta.Main.Core.Clients;

public interface IOngoingMatchesEndpointsClient : IScopedService
{
    Task<OngoingMatch> GetAsync(Guid playerId);
    Task<IEnumerable<Guid>> GetAllAsync();
}