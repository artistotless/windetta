using IdentityModel;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using System.Security.Claims;

namespace Windetta.Identity.Services;

internal sealed class CustomClaimsService : DefaultClaimsService
{
    public CustomClaimsService(
        IProfileService profile,
        ILogger<DefaultClaimsService> logger) : base(profile, logger)
    {
    }

    /// <summary>
    /// Returns claims for an access token.
    /// </summary>
    /// <param name="subject">The subject.</param>
    /// <param name="resourceResult">The validated resource result</param>
    /// <param name="request">The raw request.</param>
    /// <returns>
    /// Claims for the access token
    /// </returns>
    public override async Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, ResourceValidationResult resourceResult, ValidatedRequest request)
    {
        var baseClaims = await base.GetAccessTokenClaimsAsync(subject, resourceResult, request);

        var claims = new List<Claim>(baseClaims);

        claims.RemoveAll(x =>
        x.Type.Equals(JwtClaimTypes.AuthenticationMethod) ||
        x.Type.Equals(JwtClaimTypes.IdentityProvider));

        if (subject is null)
            return claims;

        var roleClaim = subject.FindFirst(JwtClaimTypes.Role);

        if (roleClaim is not null)
            claims.Add(roleClaim);

        return claims;
    }
}
