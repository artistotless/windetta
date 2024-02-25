using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Windetta.Common.Configuration;

public static class Extensions
{
    public static TModel GetOptions<TModel>(this IConfiguration configuration, string section) where TModel : new()
    {
        var model = new TModel();

        configuration.GetSection(section).Bind(model);

        return model;
    }

    public static void ConfigureComponentLaunchSettings(this WebApplicationBuilder builder)
    {
        var configSection = builder.Configuration
            .GetSection(nameof(ComponentLaunchSettings));

        builder.Services.Configure<ComponentLaunchSettings>(configSection);

        string? httpsUrl = configSection.GetValue<string>("httpsUrl");
        string? httpUrl = configSection.GetValue<string>("httpUrl");

        if (httpsUrl != null && httpUrl != null)
            builder.WebHost.UseUrls(httpsUrl, httpUrl);
    }
}
