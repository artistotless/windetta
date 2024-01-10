using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;

namespace Windetta.Main.Infrastructure.SignalR;

public class CreateLobbyRequestDto
{
    public Guid GameId { get; init; }
    public Bet Bet { get; init; }
    public bool Private { get; init; }
    public PluginSetDto? AutoReadyStrategy { get; init; }
    public PluginSetDto? AutoDisposeStrategy { get; init; }
    public IEnumerable<PluginSetDto>? JoinFilters { get; init; }
}
