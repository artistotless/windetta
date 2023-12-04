using Windetta.Common.Types;

namespace Windetta.Main.Core.MatchHubs;

public interface IMatchHubUsersAssociations : ISingletonService
{
    public Guid? GetHubId(Guid userId);
    internal void Set(Guid hubId, Guid userId);
    internal void Unset(Guid userId);
    internal void Unset(IEnumerable<Guid> userIds);
}
