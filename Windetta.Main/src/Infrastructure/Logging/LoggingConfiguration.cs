namespace Windetta.Main.Infrastructure.Logging;

public static class LoggingConfiguration
{
    public static void ConfigureAddLogging(WebApplicationBuilder builder)
    {
        var config = builder.Configuration.GetSection("Logging");

        builder.Logging.AddConfiguration(config);
        builder.Logging.AddConsole();
    }
}
