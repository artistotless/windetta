using Microsoft.AspNetCore.Identity;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Extensions;
using Windetta.Identity.Messages.Requests;

namespace Windetta.Identity.Handlers;

public class RegisterHandler : IRequestHandler<LocalRegisterRequest>
{
    private readonly UserManager<User> _userManager;

    public RegisterHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task HandleAsync(LocalRegisterRequest request)
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
    }

    private static void Validate(LocalRegisterRequest request)
    {
        if (request.Email is null)
            throw new ArgumentNullException(nameof(request.Email));

        if (request.Password is null)
            throw new ArgumentNullException(nameof(request.Password));

        if (request.UserName is null)
            throw new ArgumentNullException(nameof(request.UserName));
    }
}