using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using Windetta.Main.Core.MatchHub.Dtos;
using Windetta.Main.MatchHub;

namespace Windetta.Main.Infrastructure.Data.Mongo;

public class MatchHubDbModel
{
    public string InstanceId { get; set; }

    [BsonId]
    [BsonSerializer(SerializerType = typeof(GuidSerializer))]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }
    public MatchHubState State { get; set; }
    public DateTimeOffset Created { get; set; }
    public Bet Bet { get; set; }
    public IEnumerable<string>? JoinFilters { get; set; }
    public IEnumerable<RoomDbModel> Rooms { get; set; }

    // Tournament part
    [BsonSerializer(SerializerType = typeof(GuidSerializer))]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid OrganizerId { get; set; }
    public string? Description { get; set; }
    public string? Site { get; set; }

    public static MatchHubDbModel MapFrom(IMatchHub matchHub)
    {
        TournamentMatchHub? tournamentMatchHub = matchHub as TournamentMatchHub;

        return new MatchHubDbModel()
        {
            Bet = matchHub.Bet,
            Created = matchHub.Created,
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
                State = dbModel.State,
                Bet = dbModel.Bet,
                Created = dbModel.Created,
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
                Id = dbModel.Id,
                State = dbModel.State,
                JoinFilters = dbModel.JoinFilters,
                Rooms = dbModel.Rooms.Select(RoomDbModel.MapTo)
            };
        }
    }
}
