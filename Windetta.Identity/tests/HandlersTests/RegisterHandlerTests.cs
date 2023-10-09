using Microsoft.AspNetCore.Identity;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Infrastructure.Exceptions;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Tests.Mocks;
using Windetta.Tests.Identity.Mocks;

namespace Windetta.Tests.Identity.HandlersTests;

public class RegisterHandlerTests
{
    private readonly UserStore _userStore = new();
    private readonly Mock<UserManager<User>> _userManagerMock;

    public RegisterHandlerTests()
    {
        _userManagerMock = UserManagerMockFactory.Create(_userStore.GetUsers());
    }

    [Fact]
    public void HandleAsync_CreatesNewUser()
    {
        // arrange
        var sut = new LocalRegisterHandler(_userManagerMock.Object, null);
        var request = new LocalRegisterRequest()
        {
            Email = "unique-email@gmai.com",
            Password = "password",
            UserName = "unique-username",
        };

        // act
        sut.HandleAsync(request).GetAwaiter().GetResult();

        // assert
        _userStore.DeltaUsersCount.ShouldBe(1);
    }

    [Theory]
    [InlineData("user1gmail.com", "unique-username", "DuplicateEmail")]
    [InlineData("unique-email@gmail.com", "user1", "DuplicateUserName")]
    public void HandleAsync_CreatesNewUserFail_CredentialsAlreadyTaken(
        string email, string userName, string expectedErrorMessage)
    {
        // arrange
        var sut = new LocalRegisterHandler(_userManagerMock.Object, null);
        var request = new LocalRegisterRequest()
        {
            Email = email,
            UserName = userName,
            Password = "password",
        };

        // act
        var exception = Should.Throw<IdentityException>(
            () => sut.HandleAsync(request).GetAwaiter().GetResult());

        // assert
        exception.ErrorCode.ShouldBe(expectedErrorMessage);
    }

    [Theory]
    [InlineData(null, "NotNullUserName", "NotNullPassword")]
    [InlineData("NotNullEmail", null, "NotNullPassword")]
    [InlineData("NotNullEmail", "NotNullUserName", null)]
    public void HandleAsync_ThrowArgumentNullExceptionIfArgumentsNull(
        string email, string userName, string password)
    {
        // arrange
        var request = new LocalRegisterRequest()
        {
            Email = email,
            Password = password,
            UserName = userName
        };

        // act 
        var sut = new LocalRegisterHandler(null, null);
        var exception = Should.Throw<ArgumentNullException>(
            () => sut.HandleAsync(request).GetAwaiter().GetResult());

        // assert
        exception.ShouldNotBeNull();
    }
}
