namespace Windetta.Main.Core.Lobbies.Plugins;

public class DefaultReadyStrategy : AutoReadyStrategy
{
    public DefaultReadyStrategy(TimeSpan? checkInterval = null) : base(checkInterval)
    {
    }

    protected override bool CheckReady()
    {
        return Lobby.MembersCount >= Lobby.Configuration.MaxPlayers;
    }
}
