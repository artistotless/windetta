using Microsoft.AspNetCore.Mvc;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Handlers;
using Windetta.Identity.Infrastructure.Exceptions;
using Windetta.Identity.Messages.Requests;
using Windetta.Tests.Identity.Mocks;

namespace Windetta.Tests.Identity.HandlersTests;

public class RegisterHandlerTests
{
    private readonly Register _sampleRequest;

    private const string USER_EMAIL = "test@gmail.com";
    private const string USER_PASS = "Pa55W@rd";
    private const string USER_NAME = "user1";

    public RegisterHandlerTests()
    {
        _sampleRequest = new Register()
        {
            Email = USER_EMAIL,
            Password = USER_PASS,
            UserName = USER_NAME,
        };
    }

    [Fact]
    public void HandleAsync_ReturnsOkResultType()
    {
        var userStore = new List<User>();
        var userManagerMock = UserManagerMockFactory.Create(userStore);

        var sut = new RegisterHandler(userManagerMock.Object);

        var result = sut.HandleAsync(_sampleRequest).GetAwaiter().GetResult();

        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public void HandleAsync_CreatesNewUser()
    {
        var userStore = new List<User>();
        var userManagerMock = UserManagerMockFactory.Create(userStore);

        var sut = new RegisterHandler(userManagerMock.Object);

        sut.HandleAsync(_sampleRequest).GetAwaiter().GetResult();

        userStore.Count.ShouldBe(1);
    }

    [Theory]
    [InlineData(USER_EMAIL, "username", "DuplicateEmail")]
    [InlineData("email@gmail.com", USER_NAME, "DuplicateUserName")]
    public void HandleAsync_CreatesNewUserFail_CredentialsAlreadyTaken(
        string email, string userName, string expectedErrorMessage)
    {
        var userStore = new List<User>
        {
            new User() {
                Email = email,
                UserName = userName
            }
        };

        var userManagerMock = UserManagerMockFactory.Create(userStore);

        var sut = new RegisterHandler(userManagerMock.Object);

        Should.Throw<IdentityException>(() => sut.HandleAsync(_sampleRequest).GetAwaiter().GetResult())
            .ErrorCode.ShouldBe(expectedErrorMessage);
    }

    [Theory]
    [InlineData(null, "NotNullUserName", "NotNullPassword")]
    [InlineData("NotNullEmail", null, "NotNullPassword")]
    [InlineData("NotNullEmail", "NotNullUserName", null)]
    public void HandleAsync_ThrowArgumentNullExceptionIfArgumentsNull(
        string email, string userName, string password)
    {
        var request = new Register()
        {
            Email = email,
            Password = password,
            UserName = userName
        };

        var sut = new RegisterHandler(null);

        Should.Throw<ArgumentNullException>(() => sut.HandleAsync(request).GetAwaiter().GetResult());
    }
}
