namespace Windetta.Main.MatchHubs.Strategies;

public class DefaultReadyStrategy : AutoReadyStrategy
{
    public DefaultReadyStrategy(TimeSpan? checkInterval = null) : base(checkInterval)
    {
    }

    protected override bool CheckReady()
    {
        var count = Hub.Rooms.Select(x => x.MembersCount).Sum();

        if (count >= Hub.Configuration.MaxPlayers)
        {
            return true;
        }

        return false;
    }
}
