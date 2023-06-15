namespace Windetta.Common.Authentication;

public interface IRefreshTokenService
{
    public Task<string> CreateTokenAsync(Guid userId);
    public Task RevokeTokenAsync(Guid userId, string refreshToken);
    public Task<string> RefreshTokenAsync(Guid userId, string refreshToken);
}
