using System.Security.Claims;

namespace Windetta.Common.Authentication;

public struct AuthorizationCode
{
    public Guid UserId { get; set; }
    public string Value { get; set; }
    public IEnumerable<Claim> Claims { get; set; }
}

