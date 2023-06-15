using System.Security.Claims;

namespace Windetta.Common.Authentication;

public interface IAccessTokenCreator
{
    public AccessToken Create(Guid userId, IEnumerable<Claim> claims);
}
