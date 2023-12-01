using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Windetta.Main.Rooms;
using MongoDB.Bson.Serialization.Serializers;

namespace Windetta.Main.Infrastructure.Data.Mongo;

public class RoomDbModel
{
    [BsonSerializer(SerializerType = typeof(GuidSerializer))]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }
    public IEnumerable<RoomMember> Members { get; set; }
    public uint MaxMembers { get; set; }

    public static RoomDbModel MapFrom(Room room)
    {
        return new RoomDbModel()
        {
            Id = room.Id,
            MaxMembers = room.MaxMembers,
            Members = room.Members
        };
    }

    public static Room MapTo(RoomDbModel dbModel)
    {
        return new Room(dbModel.Id, dbModel.MaxMembers, dbModel.Members);
    }
}