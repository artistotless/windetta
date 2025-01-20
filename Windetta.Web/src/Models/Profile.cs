namespace Windetta.Web.Models;

public sealed class Profile
{
    public Guid Id { get; set; }
    public string? DisplayName { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
}
