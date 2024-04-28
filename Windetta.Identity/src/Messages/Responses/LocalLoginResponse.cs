using IdentityServer4.Models;

namespace Windetta.Identity.Messages.Responses;

public struct LocalLoginResponse
{
    public AuthorizationRequest Context { get; init; }
}
