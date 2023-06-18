using Windetta.Common.Types;

namespace Windetta.Identity.Services;

public interface IRefreshTokenService : IScopedService
{
    public Task<string> CreateTokenAsync(Guid userId);
    public Task RevokeTokenAsync(Guid userId, string refreshToken);
    public Task<string> RefreshTokenAsync(Guid userId, string refreshToken);
}
