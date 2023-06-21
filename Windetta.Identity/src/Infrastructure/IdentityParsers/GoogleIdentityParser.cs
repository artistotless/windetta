using System.Security.Claims;
using Windetta.Identity.Extensions;

namespace Windetta.Identity.Infrastructure.IdentityParsers;

public class GoogleIdentityParser : BaseIdentityParser
{
    public override string ProviderName => "google";

    public override ExternalIdentity Parse(IEnumerable<Claim> claims)
    {
        var baseIdentity = base.Parse(claims);

        var nameClaim = claims.FindFirst(ClaimTypes.Name);

        baseIdentity.DisplayName = nameClaim is null ? null : nameClaim.Value;

        return baseIdentity;
    }
}