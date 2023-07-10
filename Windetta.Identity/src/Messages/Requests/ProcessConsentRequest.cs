using IdentityServer4.Models;
using IdentityServer4.Services;
using Windetta.Common.IdentityServer;
using Windetta.Common.Messages;
using Windetta.Identity.Extensions;
using Windetta.Identity.Messages.Responses;
using Windetta.Identity.Models;

namespace Windetta.Identity.Messages.Requests;

internal sealed class ProcessConsentRequest : IRequest<ProcessConsentResult>
{
    public ConsentInputModel InputModel { get; set; }
}

internal sealed class ProcessConsentHandler : IRequestHandler<ProcessConsentRequest, ProcessConsentResult>
{
    private readonly IIdentityServerInteractionService _interaction;

    public ProcessConsentHandler(IIdentityServerInteractionService interaction)
    {
        _interaction = interaction;
    }

    public async Task<ProcessConsentResult> HandleAsync(ProcessConsentRequest request)
    {
        var model = request.InputModel;
        var result = new ProcessConsentResult();

        // validate return url is still valid
        var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
        if (context == null)
            return result;

        result.IsNativeClient = context.IsNativeClient();

        ConsentResponse grantedConsent;

        // user clicked 'yes' - validate the data
        if (model?.Button == "yes")
        {
            grantedConsent = ProcessConsentFromModel(model);
        }
        // user clicked 'no' - send back the standard 'access_denied' response
        else
        {
            grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };
        }

        if (grantedConsent is not null)
        {
            // communicate outcome of consent back to identityserver
            await _interaction.GrantConsentAsync(context, grantedConsent);

            // indicate that's it ok to redirect back to authorization endpoint
            result.RedirectUri = model.ReturnUrl;
        }
        else
        {
            result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
        }

        return result;
    }

    private ConsentResponse ProcessConsentFromModel(ConsentInputModel model)
    {
        // if the user consented to some scope, build the response model
        if (model.ScopesConsented != null && model.ScopesConsented.Any())
        {
            var scopes = model.ScopesConsented.ToList();

            if (!ConsentOptions.EnableOfflineAccess)
                scopes.RemoveAll(x => x != IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess);

            return new ConsentResponse
            {
                RememberConsent = model.RememberConsent,
                ScopesValuesConsented = scopes.ToArray(),
                Description = model.Description
            };
        }

        return null;
    }
}