using Microsoft.AspNetCore.Authentication;

namespace Windetta.Main.Infrastructure.Config;

public sealed class RealtimeTokenOptions : AuthenticationSchemeOptions
{
    public string PublicKey { get; set; }
    public bool ValidateLifetime { get; set; }
}
