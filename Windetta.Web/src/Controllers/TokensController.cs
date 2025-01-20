using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Types;
using Windetta.Web.Clients;

namespace Windetta.Web.Controllers;

[Route("[controller]/[action]")]
public class TokensController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private const string COOKIE_HEADER = "Set-Cookie";
    public TokensController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAccessToken()
    {
        var authData = await HttpContext.AuthenticateAsync();
        var token = authData.Properties?.GetTokenValue("access_token");

        return Ok(token);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Realtime()
    {
        using var client = _clientFactory.CreateClient(ClientsNames.IdentityClient);

        var authResult = await HttpContext.AuthenticateAsync();

        if (!authResult.Succeeded)
            return Unauthorized();

        var accessToken = authResult.Properties?.GetTokenValue("access_token");

        if (string.IsNullOrEmpty(accessToken))
            return Unauthorized();

        client.SetBearerToken(accessToken);

        var tokenResponse = await client.PostAsync("/realtimeTokens", null);

        if (!tokenResponse.IsSuccessStatusCode)
            return StatusCode((int)tokenResponse.StatusCode);

        var resultToken = await tokenResponse.Content.ReadAsStringAsync();

        var response = new BaseResponse<string>(resultToken);

        Response.Headers.Remove(COOKIE_HEADER);

        if (tokenResponse.Headers.TryGetValues(COOKIE_HEADER, out var values))
            foreach (var value in values)
                Response.Headers.Append(COOKIE_HEADER, value);

        return Json(response);
    }
}

