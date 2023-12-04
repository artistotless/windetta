using Microsoft.AspNetCore.SignalR;
using Windetta.Common.Types;

namespace Windetta.Main.Infrastructure.SignalR;

public class SignalRExceptionFilter : IHubFilter, ISingletonService
{
    private const string _CLIENT_ERROR_METHOD = "onOccuredError";

    private readonly ILogger<SignalRExceptionFilter> _logger;

    public SignalRExceptionFilter(ILogger<SignalRExceptionFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask<object> InvokeMethodAsync(
        HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
    {
        try
        {
            return await next(invocationContext);
        }
        catch (Exception ex)
        {
            if (ex is WindettaException we)
            {
                await invocationContext.Hub.Clients.Caller
                    .SendAsync(_CLIENT_ERROR_METHOD, we.Message);
            }
            else
            {
                _logger.LogError(ex, $"User {invocationContext.Context.UserIdentifier} call {invocationContext.HubMethodName} method");
            }

            throw;
        }
    }
}
