using Windetta.Identity.Data.Repositories;

namespace Windetta.Identity.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokensRepository _repository;

    public RefreshTokenService(IRefreshTokensRepository repository)
    {
        _repository = repository;
    }

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
