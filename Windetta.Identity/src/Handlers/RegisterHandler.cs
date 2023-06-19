using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Extensions;
using Windetta.Identity.Messages.Requests;

namespace Windetta.Identity.Handlers;

public class RegisterHandler : IRequestHandler<Register>
{
    private readonly UserManager<User> _userManager;

    public RegisterHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> HandleAsync(Register request)
    {
        Validate(request);

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw result.Errors.FirstErrorAsException();

        return new OkResult();
    }

    private static void Validate(Register request)
    {
        if (request.Email is null)
            throw new ArgumentNullException(nameof(request.Email));

        if (request.Password is null)
            throw new ArgumentNullException(nameof(request.Password));

        if (request.UserName is null)
            throw new ArgumentNullException(nameof(request.UserName));
    }
}