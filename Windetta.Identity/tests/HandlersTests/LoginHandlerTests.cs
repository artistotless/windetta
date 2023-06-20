using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Authentication;
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
}
