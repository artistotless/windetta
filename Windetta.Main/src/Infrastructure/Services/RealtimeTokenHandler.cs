using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Windetta.Common.Types;
using Windetta.Main.Infrastructure.Config;
using Windetta.Main.Infrastructure.Security;

namespace Windetta.Main.Infrastructure.Services;

public class RealtimeTokenHandler : AuthenticationHandler<RealtimeTokenOptions>
{
    public RealtimeTokenHandler(
        IOptionsMonitor<RealtimeTokenOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Logger.LogDebug("RealtimeTokenHandler: begins");

        var tokenFromQuery = Context.Request.Query[JwtClaimTypes.AccessToken].ToString();
        var tokenFromHeader = Context.Request.Headers.Authorization.ToString()
        .Replace("Bearer ", string.Empty);

        var token = string.IsNullOrEmpty(tokenFromQuery) ? tokenFromHeader : tokenFromQuery;

        if (string.IsNullOrEmpty(token))
            return AuthenticateResult.Fail("No realtime token");

        var decodedJsonPermitToken = Convert.FromBase64String(token);

        RealtimeToken? tokenData;

        try
        {
            tokenData = JsonConvert.DeserializeObject<RealtimeToken>
                (Encoding.UTF8.GetString(decodedJsonPermitToken));
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
            Encoding.UTF8.GetBytes($"{tokenData.UserId}{tokenData.Nickname}{tokenData.Expires}"));

        var payloadHashBytes = await sha256.ComputeHashAsync(payloadStream);
        var signatureBytes = Convert.FromBase64String(tokenData.Signature);

        if (verifier.VerifyHash(payloadHashBytes, signatureBytes))
        {
            Logger.LogDebug("RealtimeTokenHandler: successfully verified");
            return AuthenticateResult.Success(CreateTicket(tokenData));
        }
        else
        {
            Logger.LogDebug("RealtimeTokenHandler: verify failed");
            return AuthenticateResult.Fail("Validation of realtime token's signature is failed");
        }
    }

    private static AuthenticationTicket CreateTicket(RealtimeToken tokenData)
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