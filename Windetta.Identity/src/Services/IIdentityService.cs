using Windetta.Common.Authentication;
using Windetta.Identity.Messages.Comands;

namespace Windetta.Identity.Services
{
    public interface IIdentityService
    {
        Task<JsonWebToken> LoginAsync(Login command);
        Task RegisterAsync(Register command);
        Task<AuthorizationCode> ExternalLoginAsync(ExternalLogin command);
        Task<JsonWebToken> ExchangeToken(AuthorizationCode code);
    }
}
