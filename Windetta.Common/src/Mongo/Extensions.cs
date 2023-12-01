using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Windetta.Common.Mongo;

public static class Extensions
{
    public static void AddMongoOptions(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var cfg = provider.GetRequiredService<IConfiguration>();

        services.Configure<MongoDbOptions>(cfg.GetSection("MongoDb"));
    }
}
