using IdentityServer4.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Messages.Events;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Tests.Mocks;
using Windetta.IdentityTests.Mocks;
using Windetta.Tests.Identity.Mocks;

namespace Windetta.Tests.Identity.HandlersTests;

public class ExternalLoginHandlerTests
{
    private readonly UserStore _userStore = new();
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IBus> _busMock;

    public ExternalLoginHandlerTests()
    {
        _userManagerMock = UserManagerMockFactory.Create(_userStore.GetUsers());
        _busMock = BusMockFactory.Create();
    }

    [Fact]
    public void HandleAsync_ReturnCorrectRedirectUrlIfPassNull()
    {
        // arrange
        var signInManagerMock = SignInManagerMockFactory.Create(_userManagerMock.Object);
        var is4InteractionMock = Mock.Of<IIdentityServerInteractionService>();
        var sut = new ExternalLoginHandler(
            signInManagerMock.Object, is4InteractionMock, _busMock.Object);
        var fixture = new Fixture();
        var request = fixture.Build<ExternalLoginRequest>()
        .Without(p => p.ReturnUrl)
        .Create();

        // act
        var result = sut.HandleAsync(request).GetAwaiter().GetResult();

        // assert
        request.ReturnUrl.ShouldBe("~/");
    }

    [Fact]
    public void HandleAsync_ShouldPublishUserCreatedEvent()
    {
        // arrange
        var signInManagerMock = SignInManagerMockFactory.Create(_userManagerMock.Object);
        var is4InteractionMock = Mock.Of<IIdentityServerInteractionService>();
        var sut = new ExternalLoginHandler(
            signInManagerMock.Object, is4InteractionMock, _busMock.Object);

        var fixture = new Fixture();
        var request = fixture.Build<ExternalLoginRequest>()
        .Without(p => p.ReturnUrl)
        .Create();

        // act
        sut.HandleAsync(request).GetAwaiter().GetResult();

        // assert
        _busMock.Verify(x => x.Publish(It.IsAny<UserCreated>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public void HandleAsync_ShouldCreateNewUserIfUserNotFound()
    {
        // arrange
        var signInManagerMock = SignInManagerMockFactory.Create(_userManagerMock.Object);
        var is4InteractionMock = Mock.Of<IIdentityServerInteractionService>();
        var sut = new ExternalLoginHandler(
            signInManagerMock.Object, is4InteractionMock, _busMock.Object);
        var fixture = new Fixture();
        var request = fixture.Build<ExternalLoginRequest>()
        .Without(p => p.ReturnUrl)
        .Create();

        // act
        sut.HandleAsync(request).GetAwaiter().GetResult();

        // assert
        _userStore.DeltaUsersCount.ShouldBe(1);
        _userManagerMock.Verify(
            x => x.CreateAsync(It.IsNotNull<User>()), Times.Once());
        _userManagerMock.Verify(
            x => x.AddLoginAsync(It.IsNotNull<User>(), It.IsNotNull<UserLoginInfo>()), Times.Once());
    }

    [Fact]
    public void HandleAsync_ShouldCall_ExternalLoginSignInAsync()
    {
        // arrange
        var signInManagerMock = SignInManagerMockFactory.Create(_userManagerMock.Object);
        var is4InteractionMock = Mock.Of<IIdentityServerInteractionService>();
        var sut = new ExternalLoginHandler(
            signInManagerMock.Object, is4InteractionMock, _busMock.Object);
        var fixture = new Fixture();
        var request = fixture.Build<ExternalLoginRequest>()
        .Without(p => p.ReturnUrl)
        .Create();

        // act
        sut.HandleAsync(request).GetAwaiter().GetResult();

        // assert
        signInManagerMock.Verify(
            x => x.ExternalLoginSignInAsync(
                It.Is<string>(x => x.Equals(request.Provider)),
                It.Is<string>(x => x.Equals(request.Identity.UniqueId)),
                It.Is<bool>(x => x == true)), Times.Once());
    }
}
