using IdentityModel;
using System.Security.Claims;
using Windetta.Identity.Extensions;
using Windetta.Identity.Services;

namespace Windetta.Identity.Infrastructure.IdentityParsers;

public class BaseClaimsProcessor : IExternaClaimsProcessor
{
    public virtual string ProviderName { get; } = "base";

    public virtual ExternalIdentity Parse(IEnumerable<Claim> claims)
    {
        var idClaim = claims.FindFirst(ClaimTypes.NameIdentifier) ??
                      claims.FindFirst(JwtClaimTypes.Subject) ??
        throw new Exception("Unable to retrieve user identifier from external authentication service.");

        var firstnameClaim = claims.FindFirst(ClaimTypes.GivenName) ??
                             claims.FindFirst(JwtClaimTypes.GivenName);

        var lastnameClaim = claims.FindFirst(ClaimTypes.Surname) ??
                            claims.FindFirst(JwtClaimTypes.FamilyName);

        var emailClaim = claims.FindFirst(ClaimTypes.Email) ??
                         claims.FindFirst(JwtClaimTypes.Email);

        var displayName = $"{firstnameClaim?.Value ?? string.Empty} {lastnameClaim?.Value ?? string.Empty}";

        return new ExternalIdentity()
        {
            UniqueId = idClaim.Value,
            DisplayName = displayName,
            Email = emailClaim?.Value
        };
    }
}

