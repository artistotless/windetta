using TonSdk.Client;
using Windetta.TonTxns.Application.Services;
using Windetta.TonTxns.Infrastructure.Services;

namespace Windetta.TonTxns.Infrastructure.Extensions;

public static class DependencyResolver
{
    public static void AddHttpTonApi(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var cfg = provider.GetRequiredService<IConfiguration>();

        services.Configure<TonClientParameters>(cfg.GetSection("TonApi"));
        services.AddScoped<IWithdrawService, HttpTonService>();
    }

    public static void AddDepositAddress(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var cfg = provider.GetRequiredService<IConfiguration>();

        var address = cfg.GetValue<string>("TonDepositAddress");
        if (address is null)
            throw new Exception("TonDepositAddress value not found in cfg");

        services.Configure<DepositAddressSource>(x => x.Address = address);
    }
}

