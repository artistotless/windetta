using Windetta.Identity.Infrastructure.IdentityParsers;

namespace Windetta.Identity.Services;

public class ExternalClaimsProcessorFactory : IExternalClaimsProcessorFactory
{
    private static Dictionary<string, IExternaClaimsProcessor> _storage = new();

    public ExternalClaimsProcessorFactory(IEnumerable<IExternaClaimsProcessor> parsers)
    {
        _storage = parsers.ToDictionary(x => x.ProviderName, x => x);
    }

    public IExternaClaimsProcessor GetParser(string provider)
    {
        if (_storage.TryGetValue(provider, out var found))
            return found;

        return new BaseClaimsProcessor();
    }
}
