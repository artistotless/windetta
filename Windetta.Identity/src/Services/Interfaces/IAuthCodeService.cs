using Windetta.Common.Authentication;
using Windetta.Common.Types;

namespace Windetta.Identity.Services;

public interface IAuthCodeService : IScopedService
{
    Task AddCodeAsync(AuthorizationCode code);
    Task<AuthorizationCode> GetCodeAsync(string code);
    Task RemoveCodeAsync(string code);

    static string GenerateCode()
    {
        return "";
    }
}