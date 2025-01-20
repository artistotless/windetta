using Windetta.Contracts;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.Lobbies.Plugins;
using Windetta.Main.Core.Rooms;

namespace Windetta.Main.Core.Lobbies;

public interface ILobbyStrategiesListener
{
    bool IsPublic { get; init; }
    DateTimeOffset CreatedAt { get; init; }
    DateTimeOffset UpdatedAt { get; set; }
    FundsInfo Bet { get; init; }
    GameConfiguration Configuration { get; }
    IEnumerable<Room> Rooms { get; }
    int MembersCount { get; }
}

public interface ILobbyReadyListener : ILobbyStrategiesListener
{
    void SetAutoReadyStrategy(IAutoReadyStrategy strategy);
    void CallReady();
}

public interface ILobbyDisposeListener : ILobbyStrategiesListener
{
    void SetDisposeStrategy(IAutoDisposeStrategy strategy);
    void CallDispose();
}