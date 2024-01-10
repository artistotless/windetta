using Windetta.Main.Core.Games;
using Windetta.Main.Core.Lobbies.Plugins;

namespace Windetta.Main.Core.Lobbies;

public record LobbyOptions
{
    public Guid InitiatorId { get; init; }
    public Guid GameId { get; init; }
    public Bet Bet { get; init; }
    public GameConfiguration GameConfiguration { get; init; }
    public IReadOnlyDictionary<string,string>? Properties { get; init; }
    public bool Private { get; init; }
    public IAutoReadyStrategy? AutoReadyStrategy { get; init; }
    public IAutoDisposeStrategy? AutoDisposeStrategy { get; init; }
    public IEnumerable<IJoinFilter>? JoinFilters { get; init; }
}

