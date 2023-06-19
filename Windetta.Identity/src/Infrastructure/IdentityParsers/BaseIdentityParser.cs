using System.Security.Claims;
using Windetta.Common.Helpers;
using Windetta.Identity.Dtos;
using Windetta.Identity.Services;

namespace Windetta.Identity.Infrastructure.IdentityParsers;

public abstract class BaseIdentityParser : IExternalIdentityParser
{
    public abstract string ProviderName { get; }

    public virtual ExternalIdentityDto Parse(ClaimsIdentity identity)
    {
        var idClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim is null)
            throw new Exception("Unable to retrieve user identifier from external authentication service.");

        return new ExternalIdentityDto()
        {
            UniqueId = idClaim.Value,
            UserName = $"user@{idClaim.Value.Cut(6)}"
        };
    }
}

