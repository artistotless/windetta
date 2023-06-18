using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;
using Windetta.Common.Authentication;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Dtos;
using Windetta.Identity.Extensions;
using Windetta.Identity.Messages.Comands;
using Windetta.Identity.Services;

namespace Windetta.Identity.Handlers;

public class ExternalLoginHandler : IRequestHandler<ExternalLogin>
{
    private readonly IAuthCodeService _authCodeService;
    private readonly IExternalIdentityParserFactory _parsersFactory;
    private readonly UserManager<User> _userManager;

    public ExternalLoginHandler(IAuthCodeService authCodeService,
        IExternalIdentityParserFactory parsersFactory,
        UserManager<User> userManager)
    {
        _authCodeService = authCodeService;
        _parsersFactory = parsersFactory;
        _userManager = userManager;
    }

    public async Task<IActionResult> HandleAsync(ExternalLogin request)
    {
        var identity = ParseIdentity(request.Provider, request.Identity);
        var user = await _userManager.FindByLoginAsync(request.Provider, identity.UniqueId);

        // If the user not found - create a new user with attached the external login
        if (user is null)
        {
            user = new User();
            var createdResult = await _userManager.CreateAsync(user);

            if (!createdResult.Succeeded)
                throw createdResult.Errors.FirstErrorAsException();

            var attachedResult = await _userManager.AddLoginAsync(user,
                new UserLoginInfo(request.Provider, identity.UniqueId, null));

            if (!attachedResult.Succeeded)
                throw attachedResult.Errors.FirstErrorAsException();
        }

        var code = await CreateAuthCodeAsync(user.Id, identity.UniqueId);

        var redirectUrl = BuildRedirectUrl(request.ReturnUrl, code);

        return new RedirectResult(redirectUrl);
    }

    /// <summary>
    /// Asynchronously creates an authorization code for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="providerKey">The key of the identity provider associated with the user.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains the generated authorization code.</returns>

    private async Task<string> CreateAuthCodeAsync(Guid userId, string providerKey)
    {
        var code = IAuthCodeService.GenerateCode();

        await _authCodeService.AddCodeAsync(new AuthorizationCode()
        {
            Claims = new Dictionary<string, string> { { ClaimTypes.Role, Roles.USER } },
            UserId = userId,
            Value = code
        });

        return code;
    }

    /// <summary>
    /// Parses the provided identity according to the given provider.
    /// </summary>
    /// <param name="provider">The name of the provider to use for parsing the identity: e.g vk, google, steam ..</param>
    /// <param name="identity">The identity to parse.</param>
    /// <returns>An instance of ExternalIdentityDto that represents the parsed identity.</returns>

    private ExternalIdentityDto ParseIdentity(string provider, IIdentity identity)
    {
        var parser = _parsersFactory.GetParser(provider);

        return parser.Parse(identity);
    }

    /// <summary>
    /// Builds a redirect URL using the provided return URL and authorization code.
    /// </summary>
    /// <param name="returnUrl">The URL to which the user should be redirected.</param>
    /// <param name="authCode">The authorization code to be included as a query parameter in the URL.</param>
    /// <returns>Returns a string representing the fully constructed URL.</returns>

    public static string BuildRedirectUrl(string returnUrl, string authCode)
    {
        var builder = new UriBuilder(returnUrl);
        builder.Query = $"{nameof(authCode)}={authCode}";

        return builder.Uri.ToString();
    }
}
