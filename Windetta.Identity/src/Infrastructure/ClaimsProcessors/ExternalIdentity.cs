namespace Windetta.Identity.Infrastructure.IdentityParsers;

public class ExternalIdentity
{
    public string UniqueId { get; set; }
    public string? ImageUrl { get; set; } = null;
    public string? Email { get; set; } = null;
    public string? DisplayName { get; set; } = null;
}
