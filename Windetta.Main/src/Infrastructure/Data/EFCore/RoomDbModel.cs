using Windetta.Main.Core.Rooms;

namespace Windetta.Main.Infrastructure.Data.EFCore;

public class RoomDbModel
{
    public ushort Index { get; set; }
    public IEnumerable<RoomMember> Members { get; set; }
    public uint MaxMembers { get; set; }

    public static RoomDbModel MapFrom(Room room)
    {
        return new RoomDbModel()
        {
            Index = room.Index,
            MaxMembers = room.MaxMembers,
            Members = room.Members
        };
    }

    public static Room MapTo(RoomDbModel dbModel)
    {
        return new Room(dbModel.Index, dbModel.MaxMembers, dbModel.Members);
    }
}