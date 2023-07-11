using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Windetta.Common.Messages;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Extensions;

namespace Windetta.Identity.Messages.Requests;

public class LocalRegisterRequest : IRequest
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [Required]
    [MaxLength(30)]
    public string UserName { get; set; }

    [MaxLength(100)]
    [Required]
    public string Password { get; set; }
}

public class LocalRegisterHandler : IRequestHandler<LocalRegisterRequest>
{
    private readonly UserManager<User> _userManager;

    public LocalRegisterHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task HandleAsync(LocalRegisterRequest request)
    {
        Validate(request);

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            DisplayName = request.UserName
        };

        (await _userManager.CreateAsync(user, request.Password))
            .HandleBadResult();

        (await _userManager.AddToRoleAsync(user, Roles.USER))
            .HandleBadResult();

        var claims = new List<Claim>()
        {
            new(JwtClaimTypes.Email, user.Email),
            new(JwtClaimTypes.GivenName, user.DisplayName),
        };

        (await _userManager.AddClaimsAsync(user, claims))
            .HandleBadResult();
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
