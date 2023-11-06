using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Windetta.Common.Configuration;
using Windetta.Common.Database;
using Windetta.Common.Options;
using Windetta.Operations.Data;

namespace Windetta.Operations.Extensions;

public static class DependencyResolver
{

    // Configure Db connection to storing users, roles, claims and so on.
    public static void AddSagasDbContext(this IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();

        var configuration = provider.GetRequiredService<IConfiguration>();

        services.Configure<MysqlSettings>(configuration.GetSection("Mysql"));

        var settings = configuration.GetOptions<MysqlSettings>("Mysql");
        var connString = settings.GetConnectionString();

        services.AddDbContext<SagasDbContext>(options => options.UseMySql(connString, new MySqlServerVersion(settings.Version),
             b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));
    }
}
