using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Authentication;
using Windetta.Common.Types;

namespace Windetta.Common.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (WindettaException we)
        {
            context.Response.StatusCode = 400;
            context.Response.Headers.Append("X-ErrorCode", we.ErrorCode);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new ErrorDto
                (we.ErrorCode, we.Message ?? we.ErrorCode.Replace("_", " ")));
        }
        catch (AuthenticationException)
        {
            context.Response.StatusCode = 200;
            context.Response.Headers.Append("X-ErrorCode", "401");
            await context.Response.WriteAsJsonAsync(new ErrorDto("401", "Authentication failure"));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Server Error {0}", e.Message);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new ErrorDto("500", e.Message));
        }
    }
}