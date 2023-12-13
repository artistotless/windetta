using Windetta.Main.Core.Domain.Games;
using Windetta.Main.Core.Domain.MatchHubs.Plugins;
using Windetta.Main.Core.Domain.Rooms;

namespace Windetta.Main.Core.Domain.MatchHubs;

public interface IHubStrategiesListener
{
    bool IsPublic { get; init; }
    DateTimeOffset CreatedAt { get; init; }
    DateTimeOffset UpdatedAt { get; set; }
    Bet Bet { get; init; }
    GameConfiguration Configuration { get; }
    IEnumerable<Room> Rooms { get; }
    int MembersCount { get; }
}

public interface IHubReadyListener : IHubStrategiesListener
{
    void SetAutoReadyStrategy(IAutoReadyStrategy strategy);
    void OnHubAutoReady();
}

public interface IHubDisposeListener : IHubStrategiesListener
{
    void SetDisposeStrategy(IAutoDisposeStrategy strategy);
    void OnHubAutoDisposed();
}