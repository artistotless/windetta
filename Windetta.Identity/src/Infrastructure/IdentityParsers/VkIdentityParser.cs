using System.Security.Principal;
using Windetta.Identity.Dtos;
using Windetta.Identity.Services;

namespace Windetta.Identity.Infrastructure.IdentityParsers;

public class VkIdentityParser : IExternalIdentityParser
{
    public string ProviderName => "vk";

    public ExternalIdentityDto Parse(IIdentity identity)
    {
        throw new NotImplementedException();
    }
}
