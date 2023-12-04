using Windetta.Main.Games;
using Windetta.Main.MatchHubs.Strategies;
using Windetta.Main.Rooms;

namespace Windetta.Main.MatchHubs;

public interface IHubStrategiesListener
{
    bool IsPublic { get; init; }
    DateTimeOffset CreatedAt { get; init; }
    DateTimeOffset UpdatedAt { get; set; }
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