using Windetta.Common.Authentication;
using Windetta.Common.Types;

namespace Windetta.Identity.Services
{
    public interface ICodeExchangeService : IScopedService
    {
        Task<JsonWebTokenBase> Exchange(string code);
    }
}
