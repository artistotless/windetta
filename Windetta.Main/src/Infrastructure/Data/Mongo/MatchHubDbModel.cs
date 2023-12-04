using MongoDB.Bson.Serialization.Attributes;
using Windetta.Main.Core.MatchHubs.Dtos;
using Windetta.Main.MatchHubs;

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

    // Tournament part
    public Guid OrganizerId { get; set; }
    public string? Description { get; set; }
    public string? Site { get; set; }

    public static MatchHubDbModel MapFrom(IMatchHub matchHub)
    {
        TournamentMatchHub? tournamentMatchHub = matchHub as TournamentMatchHub;

        return new MatchHubDbModel()
        {
            Bet = matchHub.Bet,
            Created = matchHub.CreatedAt,
            Updated = matchHub.UpdatedAt,
            GameId = matchHub.GameId,
            Id = matchHub.Id,
            State = matchHub.State,
            JoinFilters = matchHub.JoinFilters,
            OrganizerId = tournamentMatchHub is null ? default : tournamentMatchHub.OrganizerId,
            Description = tournamentMatchHub?.Description,
            Site = tournamentMatchHub?.Site,
            Rooms = matchHub.Rooms.Select(RoomDbModel.MapFrom),
        };
    }

    public static MatchHubDto MapTo(MatchHubDbModel dbModel)
    {
        if (dbModel.OrganizerId != default)
        {
            return new TournamentMatchHubDto()
            {
                OrganizerId = dbModel.OrganizerId,
                Description = dbModel.Description,
                Site = dbModel.Site,
                GameId = dbModel.GameId,
                State = dbModel.State,
                Bet = dbModel.Bet,
                Created = dbModel.Created,
                Updated = dbModel.Updated,
                Id = dbModel.Id,
                JoinFilters = dbModel.JoinFilters,
                Rooms = dbModel.Rooms.Select(RoomDbModel.MapTo),
            };
        }
        else
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
}
