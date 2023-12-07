namespace Windetta.Main.Core.MatchHubs.Plugins;

public class DefaultDisposeStrategy : AutoDisposeStrategy
{
    protected override bool CheckDispose()
    {
        return Hub.MembersCount == 0;
    }
}
