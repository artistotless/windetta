using Windetta.Common.Types;

namespace Windetta.Identity.Services;

public interface IExternalIdentityParserFactory : ISingletonService
{
    IExternalIdentityParser GetParser(string provider);
}