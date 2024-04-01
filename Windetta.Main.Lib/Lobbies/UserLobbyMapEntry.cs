namespace Windetta.Main.Core.Lobbies;

/// <summary>
/// Represents a structure for storing data about who is in which lobby
/// </summary>
public struct UserLobbyMapEntry
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Lobby ID
    /// </summary>
    public Guid LobbyId { get; set; }

    /// <summary>
    /// Which lobby room the player is in
    /// </summary>
    public ushort RoomIndex { get; set; }

    public UserLobbyMapEntry(Guid userId, Guid lobbyId, ushort roomIndex)
    {
        UserId = userId;
        LobbyId = lobbyId;
        RoomIndex = roomIndex;
    }
}
