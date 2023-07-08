using Windetta.Common.Types;

namespace Windetta.Identity.Services;

public interface IExternalClaimsProcessorFactory : ISingletonService
{
    IExternaClaimsProcessor GetParser(string provider);
}