using Windetta.Contracts;
using Windetta.Main.Core.Lobbies.Dtos;

namespace Windetta.Main.Core.Clients.Dtos;

public class CreateLobbyRequestDto
{
    public Guid GameId { get; init; }
    public FundsInfo Bet { get; init; }
    public bool Private { get; init; }
    public PluginSetDto? AutoReadyStrategy { get; init; }
    public IReadOnlyDictionary<string, string>? Properties { get; init; }
    public PluginSetDto? AutoDisposeStrategy { get; init; }
    public IEnumerable<PluginSetDto>? JoinFilters { get; init; }
}