using Windetta.Main.Core.Lobbies.Plugins;

namespace Windetta.Main.Infrastructure.Lobby.Plugins;

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