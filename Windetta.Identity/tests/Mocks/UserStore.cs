using Windetta.Identity.Domain.Entities;

namespace Windetta.Identity.Tests.Mocks;

public class UserStore
{
    public int OriginalUsersCount { get; init; }
    public int DeltaUsersCount => _users.Count - OriginalUsersCount;

    private readonly List<User> _users;

    public UserStore()
    {
        _users = new List<User>()
        {
            new User() { UserName = "user1", Email = "user1gmail.com", PasswordHash = "user1Password" },
            new User() { UserName = "user2", Email = "user2gmail.com", PasswordHash = "user2Password" },
            new User() { UserName = "user3", Email = "user3gmail.com", PasswordHash = "user3Password" },
        };

        OriginalUsersCount = _users.Count;
    }

    public List<User> GetUsers() => _users;
}
