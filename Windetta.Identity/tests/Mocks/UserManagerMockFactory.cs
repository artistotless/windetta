using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Windetta.Tests.Identity.Mocks;

internal static class UserManagerMockFactory
{
    public static Mock<UserManager<TUser>> Create<TUser>(List<TUser> ls) where TUser : IdentityUser<Guid>
    {
        var options = new IdentityOptions();
        options.User.RequireUniqueEmail = true;

        var store = new Mock<IUserStore<TUser>>();
        var manager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);

        manager.Object.UserValidators.Add(new UserValidator<TUser>());
        manager.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
        manager.Object.Options = options;

        manager.Setup(x => x.CheckPasswordAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync((TUser user, string passHash) => ls.Any(u => u.Id == user.Id && u.PasswordHash == passHash));
        manager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((string email) => ls.Find(u => u.Email == email));
        manager.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success).Callback<TUser>(x => ls.Remove(x));
        manager.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
        manager.Setup(x => x.CreateAsync(It.IsIn(ls, new UserNameComparer<TUser>()), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError()
        {
            Code = "DuplicateUserName"
        }));

        if (options.User.RequireUniqueEmail)
        {
            manager.Setup(x => x.CreateAsync(It.IsIn(ls, new UserEmailComparer<TUser>()), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError()
            {
                Code = "DuplicateEmail"
            }));
        }

        manager.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

        return manager;
    }

    internal sealed class UserEmailComparer<TUser> : IEqualityComparer<TUser> where TUser : IdentityUser<Guid>
    {
        public bool Equals(TUser? x, TUser? y)
        {
            if (x is null && y is null)
                return false;

            if (x is null || y is null)
                return false;

            return x.Email.Equals(y.Email);
        }

        public int GetHashCode([DisallowNull] TUser obj)
        {
            return base.GetHashCode();
        }
    }

    internal sealed class UserNameComparer<TUser> : IEqualityComparer<TUser> where TUser : IdentityUser<Guid>
    {
        public bool Equals(TUser? x, TUser? y)
        {
            if (x is null && y is null)
                return false;

            if (x is null || y is null)
                return false;

            return x.UserName.Equals(y.UserName);
        }

        public int GetHashCode([DisallowNull] TUser obj)
        {
            return base.GetHashCode();
        }
    }
}
