using Windetta.Common.Authentication;
using Windetta.Common.Types;

namespace Windetta.Identity.Services
{
    public interface ITokenExchangeService : IScopedService
    {
        Task<JsonWebTokenBase> Exchange(string code);
    }
}
