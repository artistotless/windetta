using System.Security.Principal;
using Windetta.Identity.Dtos;
using Windetta.Identity.Services;

namespace Windetta.Identity.Infrastructure.IdentityParsers;

public class GoogleIdentityParser : IExternalIdentityParser
{
    public string ProviderName => "google";

    public ExternalIdentityDto Parse(IIdentity identity)
    {
        throw new NotImplementedException();
    }
}