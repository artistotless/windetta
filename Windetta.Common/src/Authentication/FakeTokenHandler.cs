using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Windetta.Common.Constants;
using Windetta.Common.Helpers;
using Windetta.Common.Types;

namespace Windetta.Common.Authentication;

public sealed record FakeToken(Guid UserId, string Nickname, string[] Scopes);
public sealed class FakeTokenOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// Authenticates with this scheme in case fake authentication cannot be performed.
    /// </summary>
    public string FallbackScheme = string.Empty;
}

public class FakeTokenHandler : AuthenticationHandler<FakeTokenOptions>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IAuthenticationSchemeProvider _schemeProvider;

    public FakeTokenHandler(
        IAuthenticationService authenticationService,
        IAuthenticationSchemeProvider schemeProvider,
        IOptionsMonitor<FakeTokenOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
        _authenticationService = authenticationService;
        _schemeProvider = schemeProvider;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Logger.LogDebug("FakeTokenHandler: begins");

        AuthenticateResult result = null!;

        try
        {
            result = await HandleAsync();
        }
        catch (Exception e)
        {
            var fallBackScheme = await _schemeProvider.GetSchemeAsync(Options.FallbackScheme);

            if (fallBackScheme is null)
                return AuthenticateResult.Fail(e.Message);

            result = await _authenticationService.AuthenticateAsync(Context, Options.FallbackScheme);
        }

        return result;
    }

    private async Task<AuthenticateResult> HandleAsync()
    {
        string? token = null;

        Context.Request.Headers.TryGetValue("X-Auth", out var tokenFromCustomHeader);

        token = tokenFromCustomHeader.ToNullableString();
        token ??= Context.Request.Query[JwtClaimTypes.AccessToken].ToNullableString();
        token ??= Context.Request.Headers.Authorization.ToNullableString();

        FakeToken? tokenData = null;

        if (string.IsNullOrEmpty(token))
            throw new Exception("No X-Auth token");

        if (token.StartsWith(AuthSchemes.Bearer))
            token = token.Remove(0, 7);

        try
        {
            var fromBase64Bytes = Convert.FromBase64String(token);
            var fromBase64Text = Encoding.UTF8.GetString(fromBase64Bytes);
            tokenData = JsonConvert.DeserializeObject<FakeToken>(fromBase64Text);
        }
        catch (Exception e)
        {
            throw new Exception("Cannot deserialize fake token", e);
        }

        if (tokenData is null)
            throw new Exception("Fake token is empty");

        Logger.LogDebug("FakeTokenHandler: successfully verified");

        return AuthenticateResult.Success(CreateTicket(tokenData));
    }

    private AuthenticationTicket CreateTicket(FakeToken tokenData)
    {
        var scopeClaims = tokenData.Scopes.Select(s => new Claim(JwtClaimTypes.Scope, s));

        List<Claim> claims = [
            new Claim(JwtClaimTypes.Subject, tokenData.UserId.ToString()),
            new Claim(JwtClaimTypes.NickName, tokenData.Nickname),
        ];

        claims.AddRange(scopeClaims);

        var identity = new ClaimsIdentity(claims, nameof(RealtimeToken));

        return new AuthenticationTicket(new ClaimsPrincipal(identity), nameof(FakeToken));
    }
}