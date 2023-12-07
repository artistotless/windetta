namespace Windetta.Main.Core.MatchHubs.Plugins;

public class DefaultReadyStrategy : AutoReadyStrategy
{
    public DefaultReadyStrategy(TimeSpan? checkInterval = null) : base(checkInterval)
    {
    }

    protected override bool CheckReady()
    {
        return Hub.MembersCount >= Hub.Configuration.MaxPlayers;
    }
}
