using Windetta.Contracts;

namespace Windetta.Main.Core.Lobbies.Dtos;

public class CreateLobbyDto
{
    public Guid GameId { get; init; }
    public FundsInfo Bet { get; init; }
    public uint Teams { get; set; }
    public uint Slots { get; set; }
    public bool Private { get; init; }
    public PluginSetDto? AutoReadyStrategy { get; init; }
    public IReadOnlyDictionary<string, string>? Properties { get; init; }
    public PluginSetDto? AutoDisposeStrategy { get; init; }
    public IEnumerable<PluginSetDto>? JoinFilters { get; init; }
}