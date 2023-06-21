using System.Security.Claims;
using Windetta.Common.Types;
using Windetta.Identity.Infrastructure.IdentityParsers;

namespace Windetta.Identity.Services;

public interface IExternalIdentityParser : ISingletonService
{
    string ProviderName { get; }
    ExternalIdentity Parse(IEnumerable<Claim> claims);
}