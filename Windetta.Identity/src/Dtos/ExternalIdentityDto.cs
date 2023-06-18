namespace Windetta.Identity.Dtos;

public class ExternalIdentityDto
{
    public string UniqueId { get; set; }
    public string? UserName { get; set; } = null;
    public string? ImageUrl { get; set; } = null;
    public string? ProfileLink { get; set; } = null;
}
