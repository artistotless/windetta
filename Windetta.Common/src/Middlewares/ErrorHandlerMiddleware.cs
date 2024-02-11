using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        catch (WindettaException e)
        {
            context.Response.StatusCode = 400;
            context.Response.Headers.Append("X-ErrorCode", e.ErrorCode);
            await context.Response.WriteAsync(e.Message ?? e.ErrorCode.Replace("_", " "));
        }
        catch (Exception e)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Uknown server error");
            _logger.LogError(e, "Server Error {0}", e.Message);
        }
    }
}