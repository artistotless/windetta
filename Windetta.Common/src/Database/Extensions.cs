using Windetta.Common.Options;

namespace Windetta.Common.Database;

public static class Extensions
{
    public static string GetConnectionString(this MysqlSettings settings)
    => string.Format("server={0};port={1};user={2};password={3};database={4}",
        settings.Server, settings.Port, settings.User, settings.Password, settings.DbName);
}
