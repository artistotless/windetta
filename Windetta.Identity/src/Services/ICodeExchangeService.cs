using Windetta.Common.Authentication;

namespace Windetta.Identity.Services;

public abstract class CodeExchangeService
{
    private readonly IAuthCodeService _authCodeService;
    private readonly JsonWebTokenService _jwtService;


    public async virtual Task<JsonWebToken> Exchange(string code)
    {
        var authCode = await _authCodeService.GetCodeAsync(code);
        var accessToken = _jwtService.Create(authCode.Claims);
        var refreshToken = _refreshTokenService.CreateTokenAsync(authCode.UserId);

        var jwt = _jwtService.

        return null;
    }
}
