using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Identity.Handlers;

public interface IRequestHandler<TRequest> : IScopedService where TRequest : IRequest
{
    Task<IActionResult> HandleAsync(TRequest request);
}
