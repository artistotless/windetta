using MongoDB.Bson.Serialization.Attributes;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Core.MatchHubs.Dtos;

namespace Windetta.Main.Infrastructure.Data.Mongo;

public class MatchHubDbModel
{
    public string InstanceId { get; set; }

    [BsonId]
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
            Created = matchHub.CreatedAt,
            Updated = matchHub.UpdatedAt,
            GameId = matchHub.GameId,
            Id = matchHub.Id,
            State = matchHub.State,
            JoinFilters = matchHub.JoinFilters,
            Rooms = matchHub.Rooms.Select(RoomDbModel.MapFrom),
        };
    }

    public static MatchHubDto MapTo(MatchHubDbModel dbModel)
    {
        return new MatchHubDto()
        {
            Bet = dbModel.Bet,
            Created = dbModel.Created,
            Updated = dbModel.Updated,
            Id = dbModel.Id,
            GameId = dbModel.GameId,
            State = dbModel.State,
            JoinFilters = dbModel.JoinFilters,
            Rooms = dbModel.Rooms.Select(RoomDbModel.MapTo)
        };
    }
}
