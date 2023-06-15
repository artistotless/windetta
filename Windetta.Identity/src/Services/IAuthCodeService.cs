using Windetta.Common.Authentication;

namespace Windetta.Identity.Services;

public interface IAuthCodeService
{
    Task<AuthorizationCode> CreateCodeAsync();
    Task<AuthorizationCode> GetCodeAsync(string code);
    Task RemoveCodeAsync(string code);
}