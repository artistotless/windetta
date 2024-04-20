using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Windetta.Common.Constants;
using Windetta.Common.Types;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Tests.Mocks;
using Windetta.Tests.Identity.Mocks;

namespace Windetta.Tests.Identity.HandlersTests;

public class LoginHandlerTests
{
    private readonly UserStore _userStore = new();
    private readonly Mock<UserManager<User>> _userManagerMock;


    public LoginHandlerTests()
    {
        _userManagerMock = UserManagerMockFactory.Create(_userStore.GetUsers());
    }

    [Fact]
    public void ShouldReturnCorrectResponse()
    {
        // arrange
        var signInManagerMock = SignInManagerMock.Create(_userManagerMock.Object);
        var is4InteractionMock = Mock.Of<IIdentityServerInteractionService>();
        var sut = new LocalLoginHandler(signInManagerMock.Object, is4InteractionMock);
        var request = new LocalLoginRequest()
        {
            Email = _userStore.GetUsers().First().Email!,
            Password = _userStore.GetUsers().First().PasswordHash!,
            RememberLogin = false,
            ReturnUrl = "~/"
        };

        // act
        var response = sut.HandleAsync(request).GetAwaiter().GetResult();

        // assert
        response.ShouldNotBeNull();
    }

    [Fact]
    public void ShouldThrowExceptionIfUserNotFound()
    {
        // arrange
        var signInManagerMock = SignInManagerMock.Create(_userManagerMock.Object);
        var is4InteractionMock = Mock.Of<IIdentityServerInteractionService>();
        var sut = new LocalLoginHandler(signInManagerMock.Object, is4InteractionMock);
        var request = new LocalLoginRequest()
        {
            Email = "notfounduser@gmail.com",
            Password = "somePassword"
        };

        // act
        var exception = Should.Throw<WindettaException>(() => sut.HandleAsync(request).GetAwaiter().GetResult());

        // assert
        exception.ErrorCode.ShouldBe(Errors.Identity.UserNotFound);
    }

    [Fact]
    public void ShouldThrowExceptionIfPasswordDoesNotMatch()
    {
        // arrange
        var signInManagerMock = SignInManagerMock.Create(_userManagerMock.Object);
        var is4InteractionMock = Mock.Of<IIdentityServerInteractionService>();
        var sut = new LocalLoginHandler(signInManagerMock.Object, is4InteractionMock);
        var request = new LocalLoginRequest()
        {
            Email = _userStore.GetUsers().First().Email!,
            Password = "fakepass"
        };

        // act
        var exception = Should.Throw<WindettaException>(() => sut.HandleAsync(request).GetAwaiter().GetResult());

        // assert
        exception.ErrorCode.ShouldBe(Errors.Identity.UserNotFound);
    }
}
