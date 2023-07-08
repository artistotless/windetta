using Microsoft.AspNetCore.Identity;
using Windetta.Common.Messages;
using Windetta.Identity.Domain.Entities;

namespace Windetta.Identity.Messages.Requests;

public class ExistUserWithEmailRequest : IRequest<bool>
{
    public string Email { get; set; }
}


public class ExistUserWithEmailHandler : IRequestHandler<ExistUserWithEmailRequest, bool>
{
    private readonly UserManager<User> _userManager;

    public ExistUserWithEmailHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> HandleAsync(ExistUserWithEmailRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        return user != null;
    }
}