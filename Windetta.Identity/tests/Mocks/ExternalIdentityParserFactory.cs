using System.Security.Claims;
using Windetta.Identity.Infrastructure.IdentityParsers;
using Windetta.Identity.Services;

namespace Windetta.Identity.Tests.Mocks;

internal static class ExternalIdentityParserMockFactory
{
    public static Mock<IExternalClaimsProcessorFactory> Create()
    {
        var fixture = new Fixture();
        var parserMock = new Mock<IExternaClaimsProcessor>();
        parserMock.Setup(x => x.Parse(It.IsAny<IEnumerable<Claim>>()))
            .Returns(fixture.Create<ExternalIdentity>());

        var factoryMock = new Mock<IExternalClaimsProcessorFactory>();
        factoryMock.Setup(x => x.GetParser(It.IsAny<string>()))
            .Returns(parserMock.Object);

        return factoryMock;
    }
}
