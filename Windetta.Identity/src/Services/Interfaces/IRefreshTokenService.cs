using Windetta.Common.Types;
using Windetta.Common.Helpers;

namespace Windetta.Identity.Services;

public interface IRefreshTokenService : IScopedService
{
    public Task<string> CreateTokenAsync(Guid userId);
    public Task RevokeTokenAsync(Guid userId);
    public Task<string> RefreshTokenAsync(Guid userId);

    static string GenerateToken()
    {
        var part1 = Guid.NewGuid().ToString().Cut(20).Replace("-", string.Empty);
        var part2 = Guid.NewGuid().ToString().Cut(20).Replace("-", string.Empty);

        return $"{part1}{part2}";
    }
}
