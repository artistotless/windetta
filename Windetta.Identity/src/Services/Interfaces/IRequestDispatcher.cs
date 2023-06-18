using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Identity.Services
{
    public interface IRequestDispatcher : ISingletonService
    {
        Task<IActionResult> HandleAsync(IRequest command);
    }
}
