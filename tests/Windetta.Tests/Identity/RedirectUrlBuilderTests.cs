using Windetta.Identity.Handlers;

namespace Windetta.Tests.Identity;

public class RedirectUrlBuilderTests
{
    [Fact]
    public void BuildRedirectUrl_ReturnsCorrectUrl()
    {
        var returnUrl = "https://somesite.com";
        var authCode = "someCode";
        var expected = "https://somesite.com/?authCode=someCode";

        var result = ExternalLoginHandler.BuildRedirectUrl(returnUrl, authCode);

        Assert.Equal(expected, result, StringComparer.Ordinal);
    }
}
