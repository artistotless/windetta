using Windetta.Common.Authentication;

namespace Windetta.Identity.Services;

public class CodeExchangeService : ICodeExchangeService
{
    private readonly IAuthCodeService _authCodeService;
    private readonly IJsonWebTokenBuilder _jwtBuilder;

    public CodeExchangeService(IAuthCodeService authCodeService, IJsonWebTokenBuilder jwtBuilder)
    {
        _authCodeService = authCodeService;
        _jwtBuilder = jwtBuilder;
    }

    /// <summary>
    /// Exchanges authCode to json web token
    /// </summary>
    /// <param name="code">AuhtorizationCode</param>
    /// <returns>Jwt object</returns>
    public async Task<JsonWebTokenBase> Exchange(string code)
    {
        var authCode = await _authCodeService.GetCodeAsync(code);

        return await _jwtBuilder.BuildAsync(authCode.UserId, authCode.Claims);
    }
}
