namespace Windetta.Main.Infrastructure.Security;

public static class CorsExtensions
{
    public static void ConfigureAddCors(this IServiceCollection services)
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
