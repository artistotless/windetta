namespace Windetta.Identity.Services;

public class ExternalIdentityParserFactory : IExternalIdentityParserFactory
{
    private static Dictionary<string, IExternalIdentityParser> _storage = new();

    public ExternalIdentityParserFactory(IEnumerable<IExternalIdentityParser> parsers)
    {
        _storage = parsers.ToDictionary(x => x.ProviderName, x => x);
    }

    public IExternalIdentityParser GetParser(string provider)
    {
        return _storage[provider];
    }
}
