namespace Windetta.Main.Infrastructure.Logging;

public static class Extensions
{
    public static void ConfigureAddLogging(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration.GetSection("Logging");

        builder.Logging.AddConfiguration(config);
        builder.Logging.AddConsole();
    }
}
