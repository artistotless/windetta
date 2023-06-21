using System.Security.Claims;
using Windetta.Identity.Extensions;

namespace Windetta.Identity.Infrastructure.IdentityParsers;

public class VkIdentityParser : BaseIdentityParser
{
    public override string ProviderName => "vk";

    public override ExternalIdentity Parse(IEnumerable<Claim> claims)
    {
        var baseIdentity = base.Parse(claims);

        var nameClaims = new[] { claims.FindFirst(ClaimTypes.GivenName), claims.FindFirst(ClaimTypes.Surname) };
        var imageClaim = claims.FindFirst("urn:vkontakte:photo:link");
        var fullName = $"{(nameClaims[0] is null ? "" : nameClaims[0]!.Value)} {(nameClaims[1] is null ? "" : nameClaims[1]!.Value)}";

        baseIdentity.DisplayName = string.IsNullOrWhiteSpace(fullName) ? null : fullName;

        if (imageClaim is not null)
            baseIdentity.ImageUrl = imageClaim.Value;

        return baseIdentity;
    }
}
