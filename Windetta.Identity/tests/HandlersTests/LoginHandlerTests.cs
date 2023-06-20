using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Handlers;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Tests.Mocks;
using Windetta.Tests.Identity.Mocks;

namespace Windetta.Tests.Identity.HandlersTests;

public class LoginHandlerTests
{
    private readonly List<User> _users;

    public LoginHandlerTests()
    {
        _users = UserStore.GetStore();
    }

    [Fact]
    public void HandleAsync_ReturnJwt()
    {

        var tokenBuilderMock = JsonWebTokenBuilderFactory.Create();
        var userManagerMock = UserManagerMockFactory.Create(_users);
        var request = new Login() { Email = "user@gmail.com", Password = "p@55word" };

        var sut = new LoginHandler(userManagerMock.Object, tokenBuilderMock.Object);

        var result = sut.HandleAsync(request).GetAwaiter().GetResult();

        //tokenBuilderMock.Verify(x => x.BuildAsync(It.IsAny(),),)
    }
}
