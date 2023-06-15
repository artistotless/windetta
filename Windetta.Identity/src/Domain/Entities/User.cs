using Microsoft.AspNetCore.Identity;

namespace Windetta.Identity.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public User()
    {
        Id = Guid.NewGuid();

        if (Email is not null)
        {
            UserName = Email.Substring(0, Email.IndexOf('@'));
        }
    }
}
