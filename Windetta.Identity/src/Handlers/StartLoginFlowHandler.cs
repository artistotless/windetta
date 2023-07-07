using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Windetta.Common.IdentityServer;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Models;

namespace Windetta.Identity.Handlers;

public class StartLoginFlowHandler : IRequestHandler<StartLoginFlowRequest, LoginViewModel>
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IClientStore _clientStore;

    public StartLoginFlowHandler(
        IIdentityServerInteractionService interaction,
        IAuthenticationSchemeProvider schemeProvider,
        IClientStore clientStore)
    {
        _interaction = interaction;
        _schemeProvider = schemeProvider;
        _clientStore = clientStore;
    }

    public async Task<LoginViewModel> HandleAsync(StartLoginFlowRequest request)
    {
        var authContext = await _interaction.GetAuthorizationContextAsync(request.ReturnUrl);
        if (authContext?.IdP != null && await _schemeProvider.GetSchemeAsync(authContext.IdP) != null)
        {
            var local = authContext.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

            // this is meant to short circuit the UI and only trigger the one external IdP
            var vm = new LoginViewModel
            {
                EnableLocalLogin = local,
                ReturnUrl = request.ReturnUrl,
                Username = authContext?.LoginHint,
            };

            if (!local)
            {
                vm.ExternalProviders = new[] {
                    new ExternalProvider {
                        AuthenticationScheme = authContext.IdP
                    }
                };
            }

            return vm;
        }

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            })
            .ToList();

        var allowLocal = true;
        if (authContext?.Client.ClientId != null)
        {
            var client = await _clientStore.FindEnabledClientByIdAsync(authContext.Client.ClientId);
            if (client != null)
            {
                allowLocal = client.EnableLocalLogin;

                if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    providers = providers.Where(provider =>
                    client.IdentityProviderRestrictions
                    .Contains(provider.AuthenticationScheme)).ToList();
            }
        }

        return new LoginViewModel
        {
            AllowRememberLogin = AccountOptions.AllowRememberLogin,
            EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
            ReturnUrl = request.ReturnUrl,
            Username = authContext?.LoginHint,
            ExternalProviders = providers.ToArray()
        };
    }
}
