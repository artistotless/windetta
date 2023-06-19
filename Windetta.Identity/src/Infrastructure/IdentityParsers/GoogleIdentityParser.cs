using System.Security.Claims;
using Windetta.Identity.Dtos;

namespace Windetta.Identity.Infrastructure.IdentityParsers;

public class GoogleIdentityParser : BaseIdentityParser
{
    public override string ProviderName => "google";

    public override ExternalIdentityDto Parse(ClaimsIdentity identity)
    {
        var baseIdentity = base.Parse(identity);

        var nameClaim = identity.FindFirst(ClaimTypes.Name);

        baseIdentity.DisplayName = nameClaim is null ? null : nameClaim.Value;

        return baseIdentity;
    }
}