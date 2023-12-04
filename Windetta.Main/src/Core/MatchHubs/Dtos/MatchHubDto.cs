using Windetta.Main.MatchHubs;
using Windetta.Main.Rooms;

namespace Windetta.Main.Core.MatchHubs.Dtos;

public class MatchHubDto
{
    public Guid Id { get; init; }
    public Guid GameId { get; init; }
    public MatchHubState State { get; init; }
    public DateTimeOffset Created { get; init; }
    public DateTimeOffset Updated { get; init; }
    public Bet Bet { get; init; }
    public IEnumerable<string>? JoinFilters { get; init; }
    public IEnumerable<Room> Rooms { get; init; }

    public MatchHubDto() { }

    public MatchHubDto(IMatchHub mapFrom)
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

