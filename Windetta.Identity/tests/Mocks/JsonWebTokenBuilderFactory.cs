using Windetta.Common.Authentication;
using Windetta.Identity.Services;

namespace Windetta.Identity.Tests.Mocks;

internal static class JsonWebTokenBuilderFactory
{
    public static Mock<IJsonWebTokenBuilder> Create()
    {
        var fixture = new Fixture();
        var mock = new Mock<IJsonWebTokenBuilder>();
        mock.Setup(x => x.BuildAsync(It.IsAny<Guid>(), It.IsAny<IDictionary<string, string>>()))
            .ReturnsAsync(fixture.Create<JsonWebTokenBase>());

        return mock;
    }
}
