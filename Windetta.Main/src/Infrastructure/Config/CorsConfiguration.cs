namespace Windetta.Main.Infrastructure.Config;

public static class CorsConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        services.AddCors(o => o.AddPolicy("allow_any_origins", builder =>
        {
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
            builder.SetIsOriginAllowed((host) => true);
            builder.AllowCredentials();
        }));
    }
}
