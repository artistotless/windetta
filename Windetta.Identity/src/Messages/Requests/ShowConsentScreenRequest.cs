using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Windetta.Common.IdentityServer;
using Windetta.Common.Messages;
using Windetta.Identity.Models;

namespace Windetta.Identity.Messages.Requests;

public class ShowConsentScreenRequest : IRequest<ConsentViewModel>
{
    public string? ReturnUrl { get; set; }
    public ConsentInputModel? Consent { get; set; }
}

public class ShowConsentScreenHandler : IRequestHandler<ShowConsentScreenRequest, ConsentViewModel>
{
    private readonly IIdentityServerInteractionService _interaction;

    public ShowConsentScreenHandler(IIdentityServerInteractionService interaction)
    {
        _interaction = interaction;
    }

    public async Task<ConsentViewModel?> HandleAsync(ShowConsentScreenRequest request)
    {
        var context = await _interaction.GetAuthorizationContextAsync(request.ReturnUrl);

        if (context is not null)
            return CreateConsentViewModel(request.ReturnUrl, context);

        throw new Exception("An attempt by the user to access the consent page occurred, but it was not possible");
    }

    private ConsentViewModel CreateConsentViewModel(string returnUrl,
        AuthorizationRequest context, ConsentInputModel? consent = null)
    {
        var vm = new ConsentViewModel
        {
            RememberConsent = consent?.RememberConsent ?? true,
            ScopesConsented = consent?.ScopesConsented ?? Enumerable.Empty<string>(),
            Description = consent?.Description,
            ReturnUrl = returnUrl,
            ClientName = context.Client.ClientName ?? context.Client.ClientId,
            ClientUrl = context.Client.ClientUri,
            ClientLogoUrl = context.Client.LogoUri,
            AllowRememberConsent = context.Client.AllowRememberConsent
        };

        vm.IdentityScopes = context.ValidatedResources.Resources.IdentityResources
            .Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || consent == null)).ToArray();

        var apiScopes = new List<ScopeViewModel>();

        foreach (var parsedScope in context.ValidatedResources.ParsedScopes)
        {
            var apiScope = context.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);

            if (apiScope != null)
            {
                var scopeVm = CreateScopeViewModel(parsedScope, apiScope, vm.ScopesConsented.Contains(parsedScope.RawValue));

                apiScopes.Add(scopeVm);
            }
        }
        if (ConsentOptions.EnableOfflineAccess && context.ValidatedResources.Resources.OfflineAccess)
        {
            var scopeView = GetOfflineAccessScope(vm.ScopesConsented.Contains(
                IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess) || consent == null);

            apiScopes.Add(scopeView);
        }

        vm.ApiScopes = apiScopes;

        return vm;
    }

    #region private helpers for creation view models 
    private static ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
    {
        var displayName = apiScope.DisplayName ?? apiScope.Name;

        if (!String.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
            displayName += ":" + parsedScopeValue.ParsedParameter;

        return new ScopeViewModel
        {
            Value = parsedScopeValue.RawValue,
            DisplayName = displayName,
            Description = apiScope.Description,
            Emphasize = apiScope.Emphasize,
            Required = apiScope.Required,
            Checked = check || apiScope.Required
        };
    }

    private static ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
        => new ScopeViewModel
        {
            Value = identity.Name,
            DisplayName = identity.DisplayName ?? identity.Name,
            Description = identity.Description,
            Emphasize = identity.Emphasize,
            Required = identity.Required,
            Checked = check || identity.Required
        };

    private static ScopeViewModel GetOfflineAccessScope(bool check)
        => new ScopeViewModel
        {
            Value = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
            DisplayName = ConsentOptions.OfflineAccessDisplayName,
            Description = ConsentOptions.OfflineAccessDescription,
            Emphasize = true,
            Checked = check
        };

    #endregion
}