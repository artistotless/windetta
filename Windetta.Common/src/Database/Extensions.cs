using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Windetta.Common.Configuration;
using Windetta.Common.Options;

namespace Windetta.Common.Database;

public static class Extensions
{
    public static void AddMysqlDbContext<T>(this IServiceCollection services, Assembly assembly) where T : DbContext
    {
        using var provider = services.BuildServiceProvider();

        var configuration = provider.GetRequiredService<IConfiguration>();

        services.Configure<MysqlSettings>(configuration.GetSection("Mysql"));

        var settings = configuration.GetOptions<MysqlSettings>("Mysql");
        var connString = settings.GetConnectionString();

        services.AddDbContext<T>(options => options.UseMySql(connString, new MySqlServerVersion(settings.Version),
             b => b.MigrationsAssembly(assembly.FullName)));
    }

    public static string GetConnectionString(this MysqlSettings settings)
    => string.Format("server={0};port={1};user={2};password={3};database={4}",
        settings.Server, settings.Port, settings.User, settings.Password, settings.DbName);
}
