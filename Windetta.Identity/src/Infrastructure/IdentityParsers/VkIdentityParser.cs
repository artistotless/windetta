using System.Security.Claims;
using Windetta.Identity.Dtos;

namespace Windetta.Identity.Infrastructure.IdentityParsers;

public class VkIdentityParser : BaseIdentityParser
{
    public override string ProviderName => "vk";

    public override ExternalIdentityDto Parse(ClaimsIdentity identity)
    {
        var baseIdentity = base.Parse(identity);

        var nameClaims = new[] { identity.FindFirst(ClaimTypes.GivenName), identity.FindFirst(ClaimTypes.Surname) };
        var imageClaim = identity.FindFirst("urn:vkontakte:photo:link");
        var fullName = $"{(nameClaims[0] is null ? "" : nameClaims[0]!.Value)} {(nameClaims[1] is null ? "" : nameClaims[1]!.Value)}";

        baseIdentity.DisplayName = string.IsNullOrWhiteSpace(fullName) ? null : fullName;

        if (imageClaim is not null)
            baseIdentity.ImageUrl = imageClaim.Value;

        return baseIdentity;
    }
}
