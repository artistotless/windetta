using Windetta.Main.Core.Lobbies.Plugins;

namespace Windetta.MainTests.Mocks;

public class FullRoomsReadyStrategy : AutoReadyStrategy
{
    public FullRoomsReadyStrategy(TimeSpan? checkInterval = null) : base(checkInterval)
    {
    }

    protected override bool CheckReady()
    {
        var count = Lobby.Rooms.Select(x => x.MembersCount).Sum();

        if (count >= Lobby.Configuration.MaxPlayers)
        {
            return true;
        }

        return false;
    }
}
