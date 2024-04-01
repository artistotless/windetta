namespace Windetta.Main.Core.Lobbies.Dtos;

public class CreateLobbyDto
{
    public Guid InitiatorId { get; set; }
    public Guid GameId { get; init; }
    public Bet Bet { get; init; }
    public bool Private { get; init; }
    public PluginSetDto? AutoReadyStrategy { get; init; }
    public IReadOnlyDictionary<string, string>? Properties { get; init; }
    public PluginSetDto? AutoDisposeStrategy { get; init; }
    public IEnumerable<PluginSetDto>? JoinFilters { get; init; }
}