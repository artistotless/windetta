using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Extensions;
using Windetta.Identity.Messages.Comands;

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
        var user = new User { Email = request.Email };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw result.Errors.FirstErrorAsException();

        return new OkResult();
    }
}