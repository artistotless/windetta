using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Windetta.Common.IdentityServer;
using Windetta.Common.Messages;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Messages.Responses;

namespace Windetta.Identity.Messages.Requests;

public class LogoutRequest : IRequest<LoggedOutResponse>
{
    public string? LogoutId { get; set; }
}

public class LocalLogoutHandler : IRequestHandler<LogoutRequest, LoggedOutResponse>
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly SignInManager<User> _signInManager;

    public LocalLogoutHandler(IIdentityServerInteractionService interaction,
        SignInManager<User> signInManager)
    {
        _interaction = interaction;
        _signInManager = signInManager;
    }

    public async Task<LoggedOutResponse> HandleAsync(LogoutRequest request)
    {
        var logoutContext = await _interaction.GetLogoutContextAsync(request.LogoutId);

        await _signInManager.SignOutAsync();

        return new LoggedOutResponse()
        {
            ClientName = logoutContext.ClientName,
            PostLogoutRedirectUri = logoutContext.PostLogoutRedirectUri,
            SignOutIframeUrl = logoutContext.SignOutIFrameUrl,
            AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
            IsLocalLogout = logoutContext.ClientId is null
        };
    }
}