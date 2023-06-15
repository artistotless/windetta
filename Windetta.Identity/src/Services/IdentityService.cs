using Microsoft.AspNetCore.Identity;
using Windetta.Common.Authentication;
using Windetta.Common.Types;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Extensions;
using Windetta.Identity.Messages.Comands;

namespace Windetta.Identity.Services;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JsonWebTokenService _jwtService;
    private readonly IAuthCodeService _authCodeService;

    public IdentityService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        JsonWebTokenService jwtService,
        IAuthCodeService authCodeService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _authCodeService = authCodeService;
    }

    public async Task<JsonWebToken> LoginAsync(Login command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);

        if (user == null)
            throw new WindettaException("User with email not registered", ErrorCodes.UserNotFound);

        var result = await _signInManager.CheckPasswordSignInAsync(user, command.Password, false);

        if (!result.Succeeded)
            throw new WindettaException("Email or password invalid", ErrorCodes.UserNotFound);


        //            new Claim(ClaimTypes.Name, user.UserName)

        //return tokenHandler.WriteToken(token);

        return new JsonWebToken();
    }

    public async Task RegisterAsync(Register command)
    {
        var user = new User { Email = command.Email };

        var result = await _userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
            throw result.Errors.FirstErrorAsException();
    }

    public async Task<AuthorizationCode> ExternalLoginAsync(ExternalLogin command)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());

        if (user is not null)
            await AttachExternalLogin(user, command.Provider, command.UniqueIdentifier);

        else
        {
            var res = await _signInManager.ExternalLoginSignInAsync(command.Provider, command.UniqueIdentifier, false);

            user = new User() { }
        }


    }

    private async Task AttachExternalLogin(User user, string provider, string uniqueId)
    {
        var result = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, uniqueId, null));

        if (!result.Succeeded)
            throw new WindettaException(ErrorCodes.AttachingExternalLoginProviderFailed,
                nameof(ErrorCodes.AttachingExternalLoginProviderFailed));
    }

    public Task<JsonWebToken> ExchangeToken(AuthorizationCode code)
    {
        throw new NotImplementedException();
    }
}
