using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Identity.Handlers;

public interface IRequestHandler<TRequest, TResult> : ITransientService where TRequest : IRequest<TResult>
{
    Task<TResult> HandleAsync(TRequest request);
}

public interface IRequestHandler<TRequest> : ITransientService where TRequest : IRequest
{
    Task HandleAsync(TRequest request);
}

