namespace Windetta.Main.Core.Domain.Rooms;

public class RoomEventArg : EventArgs
{
    public Guid MemberId { get; set; }
}