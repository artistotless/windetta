using Microsoft.AspNetCore.Identity;

namespace Windetta.Identity.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public Role(string name) : base(name)
    {
        Id = Guid.NewGuid();
    }
}
