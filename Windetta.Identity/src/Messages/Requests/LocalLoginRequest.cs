using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Windetta.Common.Constants;
using Windetta.Common.Messages;
using Windetta.Common.Types;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Messages.Responses;

namespace Windetta.Identity.Messages.Requests;

public class LocalLoginRequest : IRequest<LocalLoginResponse>
{
    public string? ReturnUrl { get; set; }

    public bool RememberLogin { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [MaxLength(100)]
    [Required]
    public string Password { get; set; }
}

public class LocalLoginHandler : IRequestHandler<LocalLoginRequest, LocalLoginResponse>
{
    private readonly SignInManager<User> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;

    public LocalLoginHandler(SignInManager<User> signInManager, IIdentityServerInteractionService interaction)
    {
        _signInManager = signInManager;
        _interaction = interaction;
    }

    public async Task<LocalLoginResponse> HandleAsync(LocalLoginRequest request)
    {
        var authContext = await _interaction.GetAuthorizationContextAsync(request.ReturnUrl);

        var user = await _signInManager.UserManager.FindByEmailAsync(request.Email);

        if (user is null)
            throw new WindettaException(Errors.Identity.UserNotFound, "Username or password invalid");

        var signInResult = await _signInManager.PasswordSignInAsync(
            user, request.Password, request.RememberLogin, false);

        if (!signInResult.Succeeded)
            throw new WindettaException(Errors.Identity.UserNotFound, "Username or password invalid");

        return new LocalLoginResponse()
        {
            Context = authContext,
        };
    }
}
