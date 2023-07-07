using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Identity.Services
{
    public interface IRequestDispatcher : ISingletonService
    {
        Task<TResult> HandleAsync<TResult>(IRequest<TResult> request);
        Task HandleAsync(IRequest request);
    }
}
