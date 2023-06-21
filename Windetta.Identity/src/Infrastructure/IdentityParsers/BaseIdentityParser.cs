using System.Security.Claims;
using Windetta.Common.Helpers;
using Windetta.Identity.Extensions;
using Windetta.Identity.Services;

namespace Windetta.Identity.Infrastructure.IdentityParsers;

public abstract class BaseIdentityParser : IExternalIdentityParser
{
    public abstract string ProviderName { get; }

    public virtual ExternalIdentity Parse(IEnumerable<Claim> claims)
    {
        var idClaim = claims.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim is null)
            throw new Exception("Unable to retrieve user identifier from external authentication service.");

        return new ExternalIdentity()
        {
            UniqueId = idClaim.Value,
            UserName = $"user@{idClaim.Value.Cut(6)}"
        };
    }
}

