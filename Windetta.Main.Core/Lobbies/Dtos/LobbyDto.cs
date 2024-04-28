using Windetta.Contracts;
using Windetta.Main.Core.Rooms;

namespace Windetta.Main.Core.Lobbies.Dtos;

public class LobbyDto
{
    public Guid Id { get; init; }
    public Guid GameId { get; init; }
    public LobbyState State { get; init; }
    public DateTimeOffset Created { get; init; }
    public DateTimeOffset Updated { get; init; }
    public FundsInfo Bet { get; init; }
    public IEnumerable<string>? JoinFilters { get; init; }
    public IEnumerable<Room> Rooms { get; init; }

    public LobbyDto() { }

    public LobbyDto(ILobby mapFrom)
    {
        Id = mapFrom.Id;
        Created = mapFrom.CreatedAt;
        Updated = mapFrom.UpdatedAt;
        Bet = mapFrom.Bet;
        JoinFilters = mapFrom.JoinFilters;
        Rooms = mapFrom.Rooms;
        State = mapFrom.State;
        GameId = mapFrom.GameId;
    }
}

