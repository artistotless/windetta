using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Windetta.Tests.Identity.Mocks;

internal static class SignInManagerMockFactory
{
    public static Mock<SignInManager<TUser>> Create<TUser>(UserManager<TUser> userManager) where TUser : IdentityUser<Guid>
    {
        var mockContextAccessor = new Mock<IHttpContextAccessor>();
        var mockClaimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
        var manager = new Mock<SignInManager<TUser>>(
            userManager, mockContextAccessor.Object, mockClaimsFactory.Object, null, null, null, null);

        return manager;
    }
}
