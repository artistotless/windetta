using Microsoft.AspNetCore.Identity;

namespace Windetta.Identity.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string? DisplayName { get; set; }

    public User()
    {
        Id = Guid.NewGuid();
    }
}
