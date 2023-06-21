using AutoFixture.Kernel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Windetta.Common.Authentication;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Handlers;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Services;
using Windetta.Identity.Tests.Mocks;
using Windetta.Tests.Identity.Mocks;

namespace Windetta.Tests.Identity.HandlersTests;

public class ExternalLoginHandlerTests
{
    private readonly UserStore _userStore = new();
    private readonly List<AuthorizationCode> _codes;
    private readonly Mock<IAuthCodeService> _authCodeServiceMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IExternalIdentityParserFactory> _parserFactoryMock;

    public ExternalLoginHandlerTests()
    {
        _codes = new List<AuthorizationCode>();
        _authCodeServiceMock = AuthCodeServiceMockFactory.Create(_codes);
        _userManagerMock = UserManagerMockFactory.Create(_userStore.GetUsers());
        _parserFactoryMock = ExternalIdentityParserMockFactory.Create();
    }

    [Fact]
    public void HandleAsync_ReturnRedirectUrl()
    {
        // arrange
        _userManagerMock.Setup(x => x.FindByLoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_userStore.GetUsers().First());
        var handler = new ExternalLoginHandler(
            _authCodeServiceMock.Object, _parserFactoryMock.Object, _userManagerMock.Object);
        var request = new Fixture().Build<ExternalLoginRequest>()
            .With(x => x.Claims, new List<Claim>())
            .Create();

        // act
        var result = handler.HandleAsync(request).GetAwaiter().GetResult();

        // assert
        result.ShouldNotBeNull();
    }

    [Fact]
    public void HandleAsync_ShouldCreateNewUserIfUserNotFound()
    {
        var handler = new ExternalLoginHandler(
      _authCodeServiceMock.Object, _parserFactoryMock.Object, _userManagerMock.Object);
        var request = new Fixture().Build<ExternalLoginRequest>()
            .With(x => x.Claims, new List<Claim>())
            .Create();

        // act
        handler.HandleAsync(request).GetAwaiter().GetResult();

        // assert
        _userStore.DeltaUsersCount.ShouldBe(1);
        _userManagerMock.Verify(
            x => x.CreateAsync(It.IsNotNull<User>()), Times.Once());
        _userManagerMock.Verify(
            x => x.AddLoginAsync(It.IsNotNull<User>(), It.IsNotNull<UserLoginInfo>()), Times.Once());
    }
}
