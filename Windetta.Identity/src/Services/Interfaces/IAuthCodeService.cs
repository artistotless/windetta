using Windetta.Common.Authentication;
using Windetta.Common.Helpers;
using Windetta.Common.Types;

namespace Windetta.Identity.Services;

public interface IAuthCodeService : IScopedService
{
    Task AddCodeAsync(AuthorizationCode code);
    Task<AuthorizationCode> GetCodeAsync(string code);
    Task RemoveCodeAsync(string code);

    static string GenerateCode()
    {
        var part1 = Guid.NewGuid().ToString().Cut(10);
        var part2 = Guid.NewGuid().ToString().Cut(10);

        return $"{part1}{part2}";
    }
}