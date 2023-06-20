using Windetta.Common.Types;
using Windetta.Identity;
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
        // arrange
        var tokenBuilderMock = JsonWebTokenBuilderFactory.Create();
        var userManagerMock = UserManagerMockFactory.Create(_users);
        var request = new LoginRequest() { Email = "user1gmail.com", Password = "user1Password" };
        var sut = new LoginHandler(userManagerMock.Object, tokenBuilderMock.Object);

        // act
        var token = sut.HandleAsync(request).GetAwaiter().GetResult();

        // assert
        token.AccessToken.ShouldNotBeNullOrEmpty();
        token.RefreshToken.ShouldNotBeNullOrEmpty();
        token.Expires.ShouldNotBe(0);
        token.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void HandleAsync_ShouldThrowExceptionIfUserNotFound()
    {
        // arrange
        var userManagerMock = UserManagerMockFactory.Create(_users);
        var request = new LoginRequest() { Email = "notfounduser@gmail.com", Password = "somePassword" };
        var sut = new LoginHandler(userManagerMock.Object, null);

        // act
        var exception = Should.Throw<WindettaException>(() => sut.HandleAsync(request).GetAwaiter().GetResult());

        // assert
        exception.ErrorCode.ShouldBe(ErrorCodes.UserNotFound);
    }

    [Fact]
    public void HandleAsync_ShouldThrowExceptionIfPasswordDoesNotMatch()
    {
        // arrange
        var userManagerMock = UserManagerMockFactory.Create(_users);
        var request = new LoginRequest() { Email = _users.First().Email, Password = "fakepass" };
        var sut = new LoginHandler(userManagerMock.Object, null);

        // act
        var exception = Should.Throw<WindettaException>(() => sut.HandleAsync(request).GetAwaiter().GetResult());

        // assert
        exception.ErrorCode.ShouldBe(ErrorCodes.UserNotFound);
    }
}
