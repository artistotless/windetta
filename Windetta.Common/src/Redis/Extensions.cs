using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Configuration;

namespace Windetta.Common.Redis;

public static class Extensions
{
    public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetOptions<RedisSettings>("Redis");

        services.AddStackExchangeRedisCache(o =>
        {
            o.InstanceName = options.InstanceName;
            o.Configuration = options.ConnectionString;
        });
    }
}
