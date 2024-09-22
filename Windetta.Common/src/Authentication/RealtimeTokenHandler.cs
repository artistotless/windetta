using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Windetta.Common.Types;

namespace Windetta.Common.Authentication;

public class RealtimeTokenHandler : AuthenticationHandler<RealtimeTokenOptions>
{
    public RealtimeTokenHandler(
        IOptionsMonitor<RealtimeTokenOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    private const string VALIDATION_FAIL = "Validation of realtime token failed";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Logger.LogDebug("RealtimeTokenHandler: begins");

        Context.Request.Cookies.TryGetValue("rt", out string? rtFromCookie);
        var rtIdFromQuery = Context.Request.Query[JwtClaimTypes.AccessToken].ToString();
        var rtIdFromHeader = Context.Request.Headers.Authorization.ToString()
        .Replace("Bearer ", string.Empty);

        var rtId = string.IsNullOrEmpty(rtIdFromQuery) ? rtIdFromHeader : rtIdFromQuery;

        if (string.IsNullOrEmpty(rtId) || string.IsNullOrEmpty(rtFromCookie))
            return AuthenticateResult.Fail("No realtime token");

        var decodedJsonToken = Convert.FromBase64String(rtFromCookie);

        RealtimeToken? tokenData;

        try
        {
            tokenData = JsonConvert.DeserializeObject<RealtimeToken>
                (Encoding.UTF8.GetString(decodedJsonToken));
        }
        catch
        {
            return AuthenticateResult.Fail("Cannot deserialize the realtime token");
        }

        if (tokenData is null)
            return AuthenticateResult.Fail("Realtime token is empty");

        if (Options.ValidateLifetime)
        {
            Logger.LogDebug("RealtimeTokenHandler: checking lifetime");

            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > tokenData.Expires)
                return AuthenticateResult.Fail("Realtime token is expired");
        }

        var verifier = ECDsa.Create();
        var publicKeyBytes = Convert.FromBase64String(Options.PublicKey);

        verifier.ImportSubjectPublicKeyInfo(new ReadOnlySpan<byte>(publicKeyBytes), out int bytesRead);

        if (bytesRead == 0)
            return AuthenticateResult.Fail("Invalid publicKey for realtime token validation");

        Logger.LogDebug("RealtimeTokenHandler: verifying signature");

        var sha256 = SHA256.Create();
        var payloadStream = new MemoryStream(
            Encoding.UTF8.GetBytes($"{rtId}{tokenData.UserId}{tokenData.Nickname}{tokenData.Expires}"));

        var payloadHashBytes = await sha256.ComputeHashAsync(payloadStream);
        var signatureBytes = Convert.FromBase64String(tokenData.Signature);

        if (verifier.VerifyHash(payloadHashBytes, signatureBytes))
        {
            Logger.LogDebug("RealtimeTokenHandler: successfully verified");
            return AuthenticateResult.Success(CreateTicket(tokenData));
        }
        else
        {
            Logger.LogDebug("RealtimeTokenHandler: Validation of realtime token's signature is failed");
            return AuthenticateResult.Fail(VALIDATION_FAIL);
        }
    }

    private AuthenticationTicket CreateTicket(RealtimeToken tokenData)
    {
        IEnumerable<Claim> claims = [
            new Claim(JwtClaimTypes.Subject, tokenData.UserId.ToString()),
            new Claim(JwtClaimTypes.NickName, tokenData.Nickname),
            new Claim(JwtClaimTypes.Scope, "realtime"),
        ];

        var identity = new ClaimsIdentity(claims, nameof(RealtimeToken));

        return new AuthenticationTicket(new ClaimsPrincipal(identity), nameof(RealtimeToken));
    }
}