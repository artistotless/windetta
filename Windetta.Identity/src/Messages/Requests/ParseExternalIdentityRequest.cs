using Microsoft.AspNetCore.Authentication;
using Windetta.Common.Messages;
using Windetta.Identity.Infrastructure.IdentityParsers;
using Windetta.Identity.Services;

namespace Windetta.Identity.Messages.Requests;

public class ParseExternalIdentityRequest : IRequest<ExternalIdentity>
{
    public AuthenticateResult AuthResult { get; set; }
    public string Provider { get; set; }
}

public class ExternalIdentityParseHandler : IRequestHandler<ParseExternalIdentityRequest, ExternalIdentity>
{
    private readonly IExternalClaimsProcessorFactory _parsersFactory;

    public ExternalIdentityParseHandler(IExternalClaimsProcessorFactory parsersFactory)
    {
        _parsersFactory = parsersFactory;
    }

    /// <summary>
    /// Parses the provided claims according to the given provider.
    /// </summary>
    /// <returns>An instance of ExternalIdentity that represents the parsed authenticateResult.</returns>
    public Task<ExternalIdentity> HandleAsync(ParseExternalIdentityRequest request)
    {
        var principal = request.AuthResult.Principal;

        var claims = principal.Claims.AsEnumerable();
        var externalIdentity = _parsersFactory.GetParser(request.Provider)
            .Parse(claims);

        return Task.FromResult(externalIdentity);
    }
}