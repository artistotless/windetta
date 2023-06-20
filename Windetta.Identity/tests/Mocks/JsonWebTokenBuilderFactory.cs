using Windetta.Common.Authentication;
using Windetta.Identity.Services;

namespace Windetta.Identity.Tests.Mocks;

internal static class JsonWebTokenBuilderFactory
{
    public static Mock<IJsonWebTokenBuilder> Create()
    {
        var fixture = new Fixture();
        var token = fixture.Create<JsonWebToken>();
        var mock = new Mock<IJsonWebTokenBuilder>();
        mock.Setup(x => x.BuildAsync(It.IsAny<Guid>(), It.IsAny<IDictionary<string, string>>()))
            .ReturnsAsync(token);

        return mock;
    }
}
