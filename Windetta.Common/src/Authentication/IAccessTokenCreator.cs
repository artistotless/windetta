using Windetta.Common.Types;

namespace Windetta.Common.Authentication;

public interface IAccessTokenCreator : IScopedService
{
    public AccessToken Create(Guid userId, IDictionary<string, string> claimsKeyValue);
}
