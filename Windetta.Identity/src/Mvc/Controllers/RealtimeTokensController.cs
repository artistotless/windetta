using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Windetta.Common.Constants;
using Windetta.Common.Helpers;
using Windetta.Common.Types;
using Windetta.Identity.Config;
using Windetta.Identity.Domain.Entities;

namespace Windetta.Identity.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = "RequireRealtimeScope")]
public class RealtimeTokensController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly RealtimeTokenOptions _options;

    public RealtimeTokensController(UserManager<User> userManager, IOptions<RealtimeTokenOptions> options)
    {
        _userManager = userManager;
        _options = options.Value;
    }

    [Route("[controller]")]
    [HttpPost]
    public async Task<IActionResult> Create()
    {
        if (UserId is null)
            throw new WindettaException(Errors.Identity.IsAllowedOnlyForUser);

        var user = await _userManager.FindByIdAsync(UserId);

        if (user is null)
            throw new WindettaException(Errors.Identity.UserNotFound);

        var tokenId = Guid.NewGuid().Cut(10);
        var nickname = user.DisplayName;
        var expires = DateTimeOffset
            .UtcNow.AddSeconds(_options.LifetimeSeconds)
            .ToUnixTimeSeconds();

        var sha256 = SHA256.Create();
        var payloadStream = new MemoryStream(Encoding.UTF8.GetBytes($"{tokenId}{UserId}{nickname}{expires}"));
        var hash = await sha256.ComputeHashAsync(payloadStream);

        var cryptor = ECDsa.Create();
        cryptor.ImportECPrivateKey(new ReadOnlySpan<byte>
            (Convert.FromBase64String(_options.PrivateKey)), out _);

        var signature = cryptor.SignHash(hash);

        var tokenData = new RealtimeToken()
        {
            Expires = expires,
            Signature = Convert.ToBase64String(signature),
            Nickname = nickname,
            UserId = Guid.Parse(UserId)
        };

        var jsonBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(tokenData));
        var result = Convert.ToBase64String(jsonBytes);
        var uri = new Uri($"http://{HttpContext.Request.Host.Value}");
        var host = uri.Host;
        var dotIndex = host.IndexOf('.');

        Response.Cookies.Append("rt", result, new()
        {
            Domain = dotIndex < 1 ? host : host.Substring(dotIndex),
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Secure = true,
            MaxAge = TimeSpan.FromSeconds(6)
        });

        return Ok(tokenId);
    }
}
