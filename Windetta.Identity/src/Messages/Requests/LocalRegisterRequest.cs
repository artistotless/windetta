using IdentityModel;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Windetta.Common.Messages;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Extensions;
using Windetta.Identity.Messages.Events;

namespace Windetta.Identity.Messages.Requests;

public class LocalRegisterRequest : IRequest
{
    public Guid? Id { get; set; }

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
    private readonly IBus _bus;

    public LocalRegisterHandler(UserManager<User> userManager, IBus bus)
    {
        _userManager = userManager;
        _bus = bus;
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

        user.Id = request.Id ?? user.Id;

        (await _userManager.CreateAsync(user, request.Password))
            .HandleBadResult();

        (await _userManager.AddToRoleAsync(user, Roles.USER))
            .HandleBadResult();

        var claims = new List<Claim>()
        {
            new(JwtClaimTypes.NickName, user.DisplayName),
        };

        (await _userManager.AddClaimsAsync(user, claims))
            .HandleBadResult();

        await _bus.Publish<UserCreated>(new()
        {
            Id = user.Id,
            Email = user.Email,
            Role = Roles.USER,
            UserName = user.UserName,
            TimeStamp = DateTime.UtcNow,
        });
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
