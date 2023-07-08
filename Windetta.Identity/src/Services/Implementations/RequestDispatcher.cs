using Windetta.Common.Messages;
using Windetta.Identity.Messages.Requests;

namespace Windetta.Identity.Services;

public class RequestDispatcher : IRequestDispatcher
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RequestDispatcher(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    // <summary>
    /// Handles the given request asynchronously by calling the appropriate request handler based on the type of the request.
    /// </summary>
    /// <param name="request">The request to be handled.</param>
    /// <returns>A task representing the asynchronous operation, 
    /// which upon completion will yield the result of the request handling, 
    /// encapsulated as an IActionResult.</returns>

    public async Task<TResult> HandleAsync<TResult>(IRequest<TResult> request)
    {
        using var scope = _scopeFactory.CreateScope();

        // Create a generic request handler type based on the type of the request
        var handlerType = typeof(IRequestHandler<,>)
                    .MakeGenericType(request.GetType(), typeof(TResult));

        // Resolve the appropriate request handler from the service provider
        dynamic handler = scope.ServiceProvider.GetRequiredService(handlerType);

        // Explicitly cast the request to the expected type.
        dynamic castedRequest = Convert.ChangeType(request, request.GetType());

        var result = await handler.HandleAsync(castedRequest);

        return result;
    }

    public async Task HandleAsync(IRequest request)
    {
        using var scope = _scopeFactory.CreateScope();

        var handlerType = typeof(IRequestHandler<>)
                .MakeGenericType(request.GetType());

        dynamic handler = scope.ServiceProvider.GetRequiredService(handlerType);
        dynamic castedRequest = Convert.ChangeType(request, request.GetType());

        await handler.HandleAsync(castedRequest);
    }

}
