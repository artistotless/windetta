using Windetta.Common.Types;

namespace Windetta.Main.Core.Lobbies;

public interface ILobbyUsersAssociations : ISingletonService
{
    public Guid? GetLobbyId(Guid userId);
    internal protected void Set(Guid lobbyId, Guid userId);
    internal protected void Unset(Guid userId);
    internal protected void Unset(IEnumerable<Guid> userIds);
}
