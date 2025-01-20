using Microsoft.Extensions.Options;
using Windetta.Common.Configuration;

namespace Windetta.Web.Clients;

public static class Extensions
{
    public static void AddHttpClients(this IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();

        var clusterMap = sp.GetService<IOptions<ClusterMap>>();

        if (clusterMap is null)
            throw new Exception("Add a ClusterMap section in appsettings!");

        services.AddHttpClient(ClientsNames.MainClient).ConfigureHttpClient((sp, client) =>
        {
            client.BaseAddress = new Uri(clusterMap.Value.MainUrl);
        });

        services.AddHttpClient(ClientsNames.IdentityClient).ConfigureHttpClient((sp, client) =>
        {
            client.BaseAddress = new Uri(clusterMap.Value.IdentityUrl);
        });
    }
}
