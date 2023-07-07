using IdentityServer4.Models;

namespace Windetta.Identity.Messages.Responses;

public class LocalLoginResponse
{
    public AuthorizationRequest Context { get; set; }
    public string Username { get; set; }
}
