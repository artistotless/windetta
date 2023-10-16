using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Windetta.Tests.Identity.Mocks;

internal static class SignInManagerMock
{
    public static Mock<SignInManager<TUser>> Create<TUser>(UserManager<TUser> userManager) where TUser : IdentityUser<Guid>
    {
        var mockContextAccessor = new Mock<IHttpContextAccessor>();
        var mockClaimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
        var manager = new Mock<SignInManager<TUser>>(
            userManager, mockContextAccessor.Object, mockClaimsFactory.Object, null, null, null, null);

        manager.Setup(x => x.PasswordSignInAsync
        (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync((string username, string password, bool ip, bool lf) =>
            {
                var user = userManager.FindByNameAsync(username).GetAwaiter().GetResult();
                if (user is null)
                    return SignInResult.Failed;
                if (userManager.CheckPasswordAsync(user, password).GetAwaiter().GetResult())
                    return SignInResult.Success;
                else
                    return SignInResult.Failed;
            });

        return manager;
    }
}
