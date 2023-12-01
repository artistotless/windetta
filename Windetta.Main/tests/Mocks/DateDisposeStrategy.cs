using Windetta.Main.MatchHub.Strategies;

namespace Windetta.MainTests.Mocks;

public class DateDisposeStrategy : AutoDisposeStrategy
{
    private readonly DateTimeOffset _created;

    public DateDisposeStrategy(TimeSpan checkInterval) : base(checkInterval)
    {
        _created = DateTimeOffset.UtcNow;
    }

    protected override bool CheckDisposed()
    {
        if ((DateTimeOffset.UtcNow - _created).TotalSeconds > 2 && Hub.Rooms.Sum(x => x.MembersCount) <= 0)
            return true;
        else return false;
    }
}
