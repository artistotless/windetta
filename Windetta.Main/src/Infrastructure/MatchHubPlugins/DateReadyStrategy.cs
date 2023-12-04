using Windetta.Main.MatchHubs.Strategies;

namespace Windetta.Main.Infrastructure.MatchHubPlugins;

public class DateReadyStrategy : AutoReadyStrategy
{
    private readonly DateTimeOffset _created;
    public DateReadyStrategy(TimeSpan? checkInterval = null) : base(checkInterval)
    {
        _created = DateTimeOffset.UtcNow;
    }

    protected override bool CheckReady()
    {
        if ((DateTimeOffset.UtcNow - _created).TotalSeconds > 10)
            return true;
        else return false;
    }
}