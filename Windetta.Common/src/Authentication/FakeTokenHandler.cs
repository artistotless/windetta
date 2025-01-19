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
public sealed class FakeTokenOptions : AuthenticationSchemeOptions { }

public class FakeTokenHandler : AuthenticationHandler<FakeTokenOptions>
{
    public FakeTokenHandler(IOptionsMonitor<FakeTokenOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Logger.LogDebug("FakeTokenHandler: begins");

        string? token = null;

        Context.Request.Headers.TryGetValue("X-Auth", out var tokenFromCustomHeader);

        token = tokenFromCustomHeader.ToNullableString();
        token ??= Context.Request.Query[JwtClaimTypes.AccessToken].ToNullableString();
        token ??= Context.Request.Headers.Authorization.ToNullableString();

        if (string.IsNullOrEmpty(token))
            return AuthenticateResult.Fail("No X-Auth token");

        FakeToken? tokenData = null;

        if (token.StartsWith(AuthSchemes.Bearer))
            token = token.Remove(0, 7);

        try
        {
            var fromBase64Bytes = Convert.FromBase64String(token);
            var fromBase64Text = Encoding.UTF8.GetString(fromBase64Bytes);
            tokenData = JsonConvert.DeserializeObject<FakeToken>(fromBase64Text);
        }
        catch
        {
            return AuthenticateResult.Fail("Cannot deserialize the fake token");
        }

        if (tokenData is null)
            return AuthenticateResult.Fail("Fake token is empty");

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