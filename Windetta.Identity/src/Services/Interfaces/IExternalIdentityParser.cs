using System.Security.Principal;
using Windetta.Common.Types;
using Windetta.Identity.Dtos;

namespace Windetta.Identity.Services;

public interface IExternalIdentityParser : ISingletonService
{
    string ProviderName { get; }
    ExternalIdentityDto Parse(IIdentity identity);
}