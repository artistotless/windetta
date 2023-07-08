using Microsoft.AspNetCore.Identity;

namespace Windetta.Identity.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public User()
    {
        Id = Guid.NewGuid();
    }
}
