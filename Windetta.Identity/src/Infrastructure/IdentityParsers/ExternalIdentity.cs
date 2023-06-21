namespace Windetta.Identity.Infrastructure.IdentityParsers;

public class ExternalIdentity
{
    public string UniqueId { get; set; }
    public string? UserName { get; set; } = null;
    public string? ImageUrl { get; set; } = null;
    public string? ProfileLink { get; set; } = null;
    public string? DisplayName { get; set; } = null;
}
