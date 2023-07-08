using System.Security.Claims;
using Windetta.Identity.Extensions;

namespace Windetta.Identity.Infrastructure.IdentityParsers;

public class VkClaimsProcessor : BaseClaimsProcessor
{
    public override string ProviderName => "vk";

    public override ExternalIdentity Parse(IEnumerable<Claim> claims)
    {
        var baseIdentity = base.Parse(claims);

        var imageClaim = claims.FindFirst("urn:vkontakte:photo:link");

        if (imageClaim is not null)
            baseIdentity.ImageUrl = imageClaim.Value;

        return baseIdentity;
    }
}
