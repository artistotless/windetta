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

    [MaxLength(25)]
    [Required]
    public string Username { get; set; }

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

        var signInResult = await _signInManager.PasswordSignInAsync(
            request.Username, request.Password, request.RememberLogin, false);

        if (!signInResult.Succeeded)
            throw new WindettaException(Errors.Identity.UserNotFound, "Username or password invalid");

        var user = await _signInManager.UserManager.FindByNameAsync(request.Username);

        if (user == null)
            throw new WindettaException(Errors.Identity.UserNotFound, "User with username not registered");

        return new LocalLoginResponse()
        {
            Context = authContext,
            Username = user.UserName ?? string.Empty,
        };
    }
}
