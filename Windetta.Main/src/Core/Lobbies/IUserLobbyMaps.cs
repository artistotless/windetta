using Windetta.Common.Types;

namespace Windetta.Main.Core.Lobbies;

public interface IUserLobbyMaps : ISingletonService
{
    public UserLobbyMapEntry? Get(Guid userId);
    internal protected void Set(UserLobbyMapEntry entry);
    internal protected void Unset(Guid userId);
    internal protected void Unset(IEnumerable<Guid> userIds);
}