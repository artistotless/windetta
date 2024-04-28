namespace Windetta.Common.Authentication;

/// <summary>
/// Service providing user ID
/// </summary>
public interface IUserIdProvider
{
    /// <summary>
    /// User ID
    /// </summary>
    Guid UserId { get; }
}
