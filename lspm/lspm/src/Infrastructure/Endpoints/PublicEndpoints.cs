using LSPM.Core.Interfaces;

namespace LSPM.Infrastructure.Endpoints;

public static class PublicEndpoints
{
    public static void UsePublicEndpoints(this WebApplication web)
    {
        web.MapGet("api/servers", (IGameServersStore servers) =>
        {
            return Results.Ok(servers.GetAll());
        });
    }
} 