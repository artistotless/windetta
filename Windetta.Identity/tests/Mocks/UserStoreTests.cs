using Windetta.Identity.Domain.Entities;

namespace Windetta.Identity.Tests.Mocks;

public class UserStoreTests
{
    [Fact]
    public void GetStore_ShouldReturn3Users()
    {
        var store = UserStore.GetStore();

        var count = store.Count();

        count.ShouldBe(3);
    }

    [Fact]
    public void GetStore_ShouldReturnListUserType()
    {
        var store = UserStore.GetStore();

        var type = store.GetType();

        type.ShouldBe(typeof(List<User>));
    }
}
