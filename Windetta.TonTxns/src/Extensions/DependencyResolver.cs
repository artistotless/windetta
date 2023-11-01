using TonSdk.Client;
using Windetta.TonTxns.Infrastructure.Services;

namespace Windetta.TonTxns.Extensions;

public static class DependencyResolver
{
    public static void AddHttpTonApi(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var cfg = provider.GetRequiredService<IConfiguration>();

        services.Configure<TonClientParameters>(cfg.GetSection("TonApi"));
        services.AddScoped<ITonService, HttpTonService>();
    }
}
