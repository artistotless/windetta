using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Windetta.Common.Messages;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Extensions;
using Windetta.Identity.Infrastructure.IdentityParsers;

namespace Windetta.Identity.Messages.Requests;

public class ExternalLoginRequest : IRequest<AuthorizationRequest>
{
    public ExternalIdentity Identity { get; set; }
    public string Provider { get; set; }
    public string ReturnUrl { get; set; }
}

public class ExternalLoginHandler : IRequestHandler<ExternalLoginRequest, AuthorizationRequest>
{
    private readonly SignInManager<User> _signinManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IAuthenticationSchemeProvider _pr;

    public ExternalLoginHandler(SignInManager<User> signinManager, IIdentityServerInteractionService interaction, IAuthenticationSchemeProvider pr)
    {
        _signinManager = signinManager;
        _interaction = interaction;
        _pr = pr;
    }

    // TODO: Do cover by unit test
    public async Task<AuthorizationRequest> HandleAsync(ExternalLoginRequest request)
    {
        if (string.IsNullOrEmpty(request.ReturnUrl))
            request.ReturnUrl = "~/";

        var user = await _signinManager.UserManager.FindByLoginAsync(request.Provider, request.Identity.UniqueId);

        // If the user not found - create a new user with attached the external login
        if (user is null)
            user = await AutoProvisionUserAsync(request.Provider, request.Identity);

        await _signinManager.ExternalLoginSignInAsync
            (request.Provider, request.Identity.UniqueId, isPersistent: true);

        var context = await _interaction.GetAuthorizationContextAsync(request.ReturnUrl);

        return context;
    }

    /// <summary>
    /// This is an asynchronous method that auto-provisions a user in the system.
    /// </summary>
    /// <param name="provider">The name of the external login provider (e.g., Google, Facebook).</param>
    /// <param name="identity">An instance of the <see cref="ExternalIdentity"/> class, containing the user's external identity information.</param>
    /// <returns>A <see cref="User"/> as the result which represents the newly provisioned user.</returns>
    private async Task<User> AutoProvisionUserAsync(string provider, ExternalIdentity identity)
    {
        var user = new User()
        {
            UserName = identity.Email,
            Email = identity.Email,
            EmailConfirmed = false,
            DisplayName = identity.DisplayName
        };

        var createdResult = await _signinManager.UserManager.CreateAsync(user);

        if (!createdResult.Succeeded)
            throw createdResult.Errors.FirstErrorAsException();

        var attachedResult = await _signinManager.UserManager.AddLoginAsync(user,
        new UserLoginInfo(provider, identity.UniqueId, identity.DisplayName));

        var claims = new List<Claim>()
        {
            new(JwtClaimTypes.Email, identity.Email),
            new(JwtClaimTypes.GivenName, identity.DisplayName),
            new(JwtClaimTypes.Picture, identity.ImageUrl),
        };

        await _signinManager.UserManager.AddClaimsAsync(user, claims);

        if (!attachedResult.Succeeded)
            throw attachedResult.Errors.FirstErrorAsException();

        return user;
    }
}