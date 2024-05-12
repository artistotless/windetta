namespace Windetta.Common.Types;

/// <summary>
/// Used whenever to transfer to the Main service, to authorize a connection to a web socket.
/// </summary>
public class RealtimeToken
{
    /// <summary>
    /// User's displayName 
    /// </summary>
    public string Nickname { get; set; }

    /// <summary>
    /// User identifier
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// How long before the permit expires
    /// </summary>
    public long Expires { get; set; }

    /// <summary>
    /// Encrypted hash
    /// </summary>
    public string Signature { get; set; }
}