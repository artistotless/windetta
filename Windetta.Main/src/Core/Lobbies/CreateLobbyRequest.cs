using Windetta.Main.Core.Lobbies.Dtos;

namespace Windetta.Main.Core.Lobbies;

public class CreateLobbyRequest
{
    public Guid InitiatorId { get; set; }
    public Guid GameId { get; init; }
    public Bet Bet { get; init; }
    public bool Private { get; init; }
    public PluginSetDto? AutoReadyStrategy { get; init; }
    public PluginSetDto? AutoDisposeStrategy { get; init; }
    public IEnumerable<PluginSetDto>? JoinFilters { get; init; }
}