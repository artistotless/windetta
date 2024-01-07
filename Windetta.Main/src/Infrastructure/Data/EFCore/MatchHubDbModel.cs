using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Core.MatchHubs.Dtos;

namespace Windetta.Main.Infrastructure.Data.EFCore;

public record BetDbModel(int CurrencyId, ulong Amount);
public class MatchHubDbModel
{
    private string _instanceId;

    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public MatchHubState State { get; set; }

    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
    public Bet Bet { get; set; }
    public IEnumerable<string>? JoinFilters { get; set; }
    public IEnumerable<RoomDbModel> Rooms { get; set; }

    public static MatchHubDbModel MapFrom(IMatchHub matchHub)
    {
        return new MatchHubDbModel()
        {
            Bet = matchHub.Bet,
            GameId = matchHub.GameId,
            Created = matchHub.CreatedAt,
            Updated = matchHub.UpdatedAt,
            State = matchHub.State,
            Id = matchHub.Id,
            JoinFilters = matchHub.JoinFilters,
            Rooms = matchHub.Rooms.Select(RoomDbModel.MapFrom),
        };
    }

    public static MatchHubDto MapTo(MatchHubDbModel dbModel)
    {
        return new MatchHubDto()
        {
            Bet = dbModel.Bet,
            State = dbModel.State,
            GameId = dbModel.GameId,
            Created = dbModel.Created,
            Updated = dbModel.Updated,
            Id = dbModel.Id,
            JoinFilters = dbModel.JoinFilters,
            Rooms = dbModel.Rooms.Select(RoomDbModel.MapTo)
        };
    }
}
