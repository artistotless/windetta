using Windetta.Common.Types;

namespace Windetta.Main.Core.MatchHub;

public interface IMatchHubUsersAssociations : ISingletonService
{
    public Guid? GetHubId(Guid userId);
    internal void Set(Guid hubId, Guid userId);
    internal void Remove(Guid userId);
}
