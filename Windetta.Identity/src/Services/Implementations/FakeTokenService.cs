using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using Windetta.Common.Authentication;

namespace Windetta.Identity.Services;

internal sealed class FakeTokenCreationService : DefaultTokenCreationService
{
    public FakeTokenCreationService(
        ISystemClock clock,
        IKeyMaterialService keys,
        IdentityServerOptions options,
        ILogger<DefaultTokenCreationService> logger) : base(clock, keys, options, logger)
    {
    }

    /// <inheritdoc cref="DefaultTokenCreationService.CreateTokenAsync(Token)"/>
    public override Task<string> CreateTokenAsync(Token token)
    {
        Logger.LogTrace("Creating JWT fake token");

        var fakeToken = new FakeToken(Guid.Parse(token.SubjectId), "fake", token.Scopes.ToArray());

        var tokenResult = JsonConvert.SerializeObject(fakeToken);

        return Task.FromResult(tokenResult);
    }

}
