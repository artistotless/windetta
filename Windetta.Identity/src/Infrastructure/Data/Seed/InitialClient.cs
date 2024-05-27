namespace Windetta.Identity.Infrastructure.Data.Seed;

public sealed class InitialClient
{
    public string ClientId { get; set; }
    public string Description { get; set; }
    public string Secrets { get; set; }
    public bool RequireConsent { get; set; }
    public string[] RedirectUris { get; set; }
    public string[] PostLogoutRedirectUris { get; set; }
}
