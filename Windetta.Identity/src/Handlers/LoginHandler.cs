using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Windetta.Common.Types;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Messages.Requests;
using Windetta.Identity.Services;

namespace Windetta.Identity.Handlers;

public class LoginHandler : IRequestHandler<Login>
{
    private readonly UserManager<User> _userManager;
    private readonly IJsonWebTokenBuilder _jwtBuilder;

    public LoginHandler(UserManager<User> userManager, IJsonWebTokenBuilder jwtBuilder)
    {
        _userManager = userManager;
        _jwtBuilder = jwtBuilder;
    }

    public async Task<IActionResult> HandleAsync(Login request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            throw new WindettaException(ErrorCodes.UserNotFound, "User with email not registered");

        var passwordMatch = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordMatch)
            throw new WindettaException(ErrorCodes.UserNotFound, "Email or password invalid");

        var claims = new Dictionary<string, string>() {
            { ClaimTypes.Role,Roles.USER}
        };

        var jwt = await _jwtBuilder.BuildAsync(user.Id, claims);

        return new OkObjectResult(jwt);
    }
}