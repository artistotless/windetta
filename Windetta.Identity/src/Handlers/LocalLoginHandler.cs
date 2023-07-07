using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Windetta.Common.Types;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Messages.Responses;

namespace Windetta.Identity.Handlers;

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
            throw new WindettaException(ErrorCodes.UserNotFound, "Username or password invalid");

        var user = await _signInManager.UserManager.FindByNameAsync(request.Username);

        if (user == null)
            throw new WindettaException(ErrorCodes.UserNotFound, "User with username not registered");

        return new LocalLoginResponse()
        {
            Context = authContext,
            Username = user.UserName ?? string.Empty,
        };
    }
}