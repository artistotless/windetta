namespace Windetta.Main.Core.Domain.MatchHubs.Plugins;

public class DefaultDisposeStrategy : AutoDisposeStrategy
{
    protected override bool CheckDispose()
    {
        return Hub.MembersCount == 0;
    }
}
