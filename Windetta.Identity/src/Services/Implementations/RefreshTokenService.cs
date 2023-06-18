namespace Windetta.Identity.Services;

public class RefreshTokenService : IRefreshTokenService
{
    public Task<string> CreateTokenAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<string> RefreshTokenAsync(Guid userId, string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task RevokeTokenAsync(Guid userId, string refreshToken)
    {
        throw new NotImplementedException();
    }
}
