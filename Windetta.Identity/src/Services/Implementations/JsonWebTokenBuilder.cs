using Windetta.Common.Authentication;

namespace Windetta.Identity.Services;

public class JsonWebTokenBuilder : IJsonWebTokenBuilder
{
    private readonly IAccessTokenCreator _accessTokenCreator;
    private readonly IRefreshTokenService _refreshTokenService;

    public JsonWebTokenBuilder(IAccessTokenCreator accessTokenCreator, IRefreshTokenService refreshTokenService)
    {
        _accessTokenCreator = accessTokenCreator;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<JsonWebTokenBase> BuildAsync(Guid userId, IDictionary<string, string> claims)
    {
        var accessToken = _accessTokenCreator.Create(userId, claims);
        var refreshToken = await _refreshTokenService.CreateTokenAsync(userId);

        return new JsonWebToken(accessToken, refreshToken);
    }
}
