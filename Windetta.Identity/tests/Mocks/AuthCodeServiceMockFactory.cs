using Windetta.Common.Authentication;
using Windetta.Common.Types;
using Windetta.Identity.Services;

namespace Windetta.Identity.Tests.Mocks;

internal static class AuthCodeServiceMockFactory
{
    public static Mock<IAuthCodeService> Create(List<AuthorizationCode> codesStore)
    {
        var values = codesStore.Select(x => x.Value).ToList();
        var mock = new Mock<IAuthCodeService>();
        mock.Setup(x => x.AddCodeAsync(It.IsAny<AuthorizationCode>()))
            .Callback<AuthorizationCode>((c) => codesStore.Add(c));
        mock.Setup(x => x.GetCodeAsync(It.IsNotIn<string>(values)))
            .ThrowsAsync(new WindettaException(ErrorCodes.AuthCodeNotFound));
        mock.Setup(x => x.GetCodeAsync(It.IsIn<string>(values)))
            .ReturnsAsync((string v) => codesStore.First(c => c.Value == v));
        mock.Setup(x => x.RemoveCodeAsync(It.IsIn<string>(values)))
          .Callback<string>(x => codesStore.RemoveAll(c => c.Value == x));

        return mock;
    }
}
