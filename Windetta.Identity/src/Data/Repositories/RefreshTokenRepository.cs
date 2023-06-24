using Microsoft.Extensions.Caching.Distributed;

namespace Windetta.Identity.Data.Repositories;

public class RefreshTokenRepository : IRefreshTokensRepository
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _expires;

    private const string CACHE_KEY_PREFIX = "refresh:";

    public RefreshTokenRepository(IDistributedCache cache)
    {
        _cache = cache;
        _expires = TimeSpan.FromDays(7);
    }

    public Task SetAsync(Guid userId, string refreshToken)
        => _cache.SetStringAsync(WithPrefix(userId), refreshToken,
            new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = _expires });

    public Task<string?> GetAsync(Guid userId)
      => _cache.GetStringAsync(WithPrefix(userId));

    public Task RemoveAsync(Guid userId)
        => _cache.RemoveAsync(WithPrefix(userId));

    private static string WithPrefix(Guid userId)
        => $"{CACHE_KEY_PREFIX}{userId}";
}
