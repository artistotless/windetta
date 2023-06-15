using System.Security.Claims;
using Windetta.Common.Authentication;

namespace Windetta.Identity.Services;

public class JsonWebTokenService
{
    private readonly IAccessTokenCreator _accessTokenCreator;
    private readonly IRefreshTokenService _refreshTokenService;

    public JsonWebTokenService(IAccessTokenCreator accessTokenCreator, IRefreshTokenService refreshTokenService)
    {
        _accessTokenCreator = accessTokenCreator;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<JsonWebTokenBase> Build(Guid userId, IEnumerable<Claim> claims)
    {
        var accessToken = _accessTokenCreator.Create(userId, claims);
        var refreshToken = await _refreshTokenService.CreateTokenAsync(userId);

        return new JsonWebToken(accessToken, refreshToken);
    }
}