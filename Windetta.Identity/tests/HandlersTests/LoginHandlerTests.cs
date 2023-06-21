using Microsoft.AspNetCore.Identity;
using Windetta.Common.Types;
using Windetta.Identity;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Handlers;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Services;
using Windetta.Identity.Tests.Mocks;
using Windetta.Tests.Identity.Mocks;

namespace Windetta.Tests.Identity.HandlersTests;

public class LoginHandlerTests
{
    private readonly UserStore _userStore = new();
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IJsonWebTokenBuilder> _tokenBuilderMock;

    public LoginHandlerTests()
    {
        _userManagerMock = UserManagerMockFactory.Create(_userStore.GetUsers());
        _tokenBuilderMock = JsonWebTokenBuilderFactory.Create();
    }

    [Fact]
    public void HandleAsync_ReturnJwt()
    {
        // arrange
        var request = new LoginRequest() { Email = "user1gmail.com", Password = "user1Password" };
        var sut = new LoginHandler(_userManagerMock.Object, _tokenBuilderMock.Object);

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
        var request = new LoginRequest() { Email = "notfounduser@gmail.com", Password = "somePassword" };
        var sut = new LoginHandler(_userManagerMock.Object, null);

        // act
        var exception = Should.Throw<WindettaException>(() => sut.HandleAsync(request).GetAwaiter().GetResult());

        // assert
        exception.ErrorCode.ShouldBe(ErrorCodes.UserNotFound);
    }

    [Fact]
    public void HandleAsync_ShouldThrowExceptionIfPasswordDoesNotMatch()
    {
        // arrange
        var request = new LoginRequest()
        {
            Email = _userStore.GetUsers().First().Email!,
            Password = "fakepass"
        };

        var sut = new LoginHandler(_userManagerMock.Object, null);

        // act
        var exception = Should.Throw<WindettaException>(() => sut.HandleAsync(request).GetAwaiter().GetResult());

        // assert
        exception.ErrorCode.ShouldBe(ErrorCodes.UserNotFound);
    }
}
