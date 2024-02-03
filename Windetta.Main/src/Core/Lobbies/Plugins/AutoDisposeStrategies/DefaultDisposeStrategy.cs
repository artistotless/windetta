namespace Windetta.Main.Core.Lobbies.Plugins;

public class DefaultDisposeStrategy : AutoDisposeStrategy
{
    protected override bool CheckDispose()
    {
        return Lobby.MembersCount == 0;
    }
}
