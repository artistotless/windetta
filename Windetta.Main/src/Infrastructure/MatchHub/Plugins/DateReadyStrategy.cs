using Windetta.Main.Core.Domain.MatchHubs.Plugins;

namespace Windetta.Main.Infrastructure.MatchHub.Plugins;

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