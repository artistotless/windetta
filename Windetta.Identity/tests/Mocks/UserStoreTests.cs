using Windetta.Identity.Domain.Entities;

namespace Windetta.Identity.Tests.Mocks;

public class UserStoreTests
{
    private readonly UserStore _userStore = new();

    [Fact]
    public void GetStore_ShouldReturn3Users()
    {
        _userStore.OriginalUsersCount.ShouldBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void GetStore_ShouldReturnListUserType()
    {
        var type = _userStore.GetUsers().GetType();

        type.ShouldBe(typeof(List<User>));
    }
}
