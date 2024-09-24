using Microsoft.AspNetCore.Authentication;

namespace Windetta.Common.Authentication;

public sealed class RealtimeTokenOptions : AuthenticationSchemeOptions
{
    public string PublicKey { get; set; }
    public bool ValidateLifetime { get; set; }
}