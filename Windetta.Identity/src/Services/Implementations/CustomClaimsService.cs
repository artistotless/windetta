using IdentityModel;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Windetta.Identity.Domain.Entities;

namespace Windetta.Identity.Services;

internal sealed class CustomClaimsService : DefaultClaimsService
{
    private readonly RoleManager<Role> _roleManager;

    public CustomClaimsService(
        RoleManager<Role> roleManager,
        IProfileService profile,
        ILogger<DefaultClaimsService> logger) : base(profile, logger)
    {
        _roleManager = roleManager;
    }

    /// <summary>
    /// Returns claims for an access token.
    /// </summary>
    /// <param name="subject">The subject.</param>
    /// <param name="resourceResult">The validated resource result</param>
    /// <param name="request">The raw request.</param>
    /// <returns>
    /// Claims for the access token
    /// </returns>
    public override async Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, ResourceValidationResult resourceResult, ValidatedRequest request)
    {
        var baseClaims = await base.GetAccessTokenClaimsAsync(subject, resourceResult, request);
        var claims = new List<Claim>(baseClaims);

        var roleScope = baseClaims.FirstOrDefault(
            x => x.Type.Equals("scope") && x.Value.Equals(JwtClaimTypes.Role));

        var roleClaim = subject.FindFirst(JwtClaimTypes.Role);

        if (roleScope is not null && roleClaim is not null)
        {
            var exist = await _roleManager.RoleExistsAsync(roleClaim.Value);
            if (exist is true)
                claims.Add(roleClaim);
        }

        return claims;
    }
}
