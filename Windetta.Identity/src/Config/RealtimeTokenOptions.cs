namespace Windetta.Identity.Config;

public class RealtimeTokenOptions
{
    public string PrivateKey { get; set; }
    public string PublicKey { get; set; }
    public int LifetimeSeconds { get; set; }
}
