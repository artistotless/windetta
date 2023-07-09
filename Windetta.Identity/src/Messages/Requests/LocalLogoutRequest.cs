using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Windetta.Common.Messages;
using Windetta.Identity.Domain.Entities;

namespace Windetta.Identity.Messages.Requests;

public class LocalLogoutRequest : IRequest<string>
{
    public string? LogoutId { get; set; }
}

public class LocalLogoutHandler : IRequestHandler<LocalLogoutRequest, string>
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly SignInManager<User> _signInManager;

    public LocalLogoutHandler(IIdentityServerInteractionService interaction,
        SignInManager<User> signInManager)
    {
        _interaction = interaction;
        _signInManager = signInManager;
    }

    public async Task<string> HandleAsync(LocalLogoutRequest request)
    {
        var logoutContext = await _interaction.GetLogoutContextAsync(request.LogoutId);

        await _signInManager.SignOutAsync();

        if (logoutContext is null)
            return "~/";

        return logoutContext.PostLogoutRedirectUri;
    }
}