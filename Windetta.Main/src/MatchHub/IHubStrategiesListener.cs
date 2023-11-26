using Windetta.Main.Games;
using Windetta.Main.MatchHub.Strategies;
using Windetta.Main.Rooms;

namespace Windetta.Main.MatchHub;

public interface IHubStrategiesListener
{
    bool IsPublic { get; init; }
    DateTimeOffset Created { get; init; }
    Bet Bet { get; init; }
    GameConfiguration Configuration { get; }
    IEnumerable<Room> Rooms { get; }
}

public interface IHubReadyListener : IHubStrategiesListener
{
    void SetAutoReadyStrategy(AutoReadyStrategy strategy);
    void OnHubAutoReady();
}

public interface IHubDisposeListener : IHubStrategiesListener
{
    void SetDisposeStrategy(AutoDisposeStrategy strategy);
    void OnHubAutoDisposed();
}