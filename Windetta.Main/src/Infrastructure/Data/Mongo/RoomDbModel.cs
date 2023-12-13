using Windetta.Main.Core.Domain.Rooms;

namespace Windetta.Main.Infrastructure.Data.Mongo;

public class RoomDbModel
{
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