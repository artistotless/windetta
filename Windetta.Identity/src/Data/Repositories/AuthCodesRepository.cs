using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Windetta.Common.Authentication;
using Windetta.Common.Types;

namespace Windetta.Identity.Data.Repositories;

public class AuthCodesRepository : IAuthCodesRepository
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _expires;

    public AuthCodesRepository(IDistributedCache cache)
    {
        _cache = cache;
        _expires = TimeSpan.FromSeconds(20);
    }

    public async Task AddAsync(AuthorizationCode value)
    {
        string jsonString = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(value.Value, jsonString, new DistributedCacheEntryOptions()
        {
            AbsoluteExpiration = DateTimeOffset.UtcNow + _expires
        });
    }

    public async Task<AuthorizationCode> GetAsync(string key)
    {
        var value = await _cache.GetStringAsync(key);

        if (value is null)
            throw new WindettaException(ErrorCodes.AuthCodeNotFound);

        return JsonSerializer.Deserialize<AuthorizationCode>(value) ?? new AuthorizationCode();
    }

    public Task RemoveAsync(string key) => _cache.RemoveAsync(key);
}
