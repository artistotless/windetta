using Windetta.Identity.Data.Repositories;

namespace Windetta.Identity.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokensRepository _repository;
    private readonly IJsonWebTokenBuilder _jwtBuilder;

    public RefreshTokenService(IRefreshTokensRepository repository, IJsonWebTokenBuilder jwtBuilder)
    {
        _repository = repository;
        _jwtBuilder = jwtBuilder;
    }

    public async Task<string> CreateTokenAsync(Guid userId)
    {
        var token = IRefreshTokenService.GenerateToken();
        await _repository.SetAsync(userId, token);

        return token;
    }

    public async Task<string> RefreshTokenAsync(Guid userId)
        => await CreateTokenAsync(userId);

    public async Task RevokeTokenAsync(Guid userId)
        => await _repository.RemoveAsync(userId);
}
