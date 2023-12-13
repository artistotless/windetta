using Windetta.Common.Types;

namespace Windetta.Main.Core.Domain.MatchHubs;

public interface IMatchHubUsersAssociations : ISingletonService
{
    public Guid? GetHubId(Guid userId);
    internal protected void Set(Guid hubId, Guid userId);
    internal protected void Unset(Guid userId);
    internal protected void Unset(IEnumerable<Guid> userIds);
}
