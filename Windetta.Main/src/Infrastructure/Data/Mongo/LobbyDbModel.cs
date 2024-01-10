using MongoDB.Bson.Serialization.Attributes;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;

namespace Windetta.Main.Infrastructure.Data.Mongo;

public class LobbyDbModel
{
    public string InstanceId { get; set; }

    [BsonId]
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public LobbyState State { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
    public Bet Bet { get; set; }
    public IEnumerable<string>? JoinFilters { get; set; }
    public IEnumerable<RoomDbModel> Rooms { get; set; }

    public static LobbyDbModel MapFrom(ILobby lobby)
    {
        return new LobbyDbModel()
        {
            Bet = lobby.Bet,
            Created = lobby.CreatedAt,
            Updated = lobby.UpdatedAt,
            GameId = lobby.GameId,
            Id = lobby.Id,
            State = lobby.State,
            JoinFilters = lobby.JoinFilters,
            Rooms = lobby.Rooms.Select(RoomDbModel.MapFrom),
        };
    }

    public static LobbyDto MapTo(LobbyDbModel dbModel)
    {
        return new LobbyDto()
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
