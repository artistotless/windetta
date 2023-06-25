using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
//using Windetta.Identity.Extensions.IS4Mapping;

namespace Windetta.Identity.Data.Seed;

public static class IdentityServerConfigurationSeeder
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices
            .GetService<IServiceScopeFactory>()!.CreateScope();

        var provider = serviceScope.ServiceProvider;
        var dbContext = provider.GetRequiredService<ConfigurationDbContext>();

        dbContext.Database.Migrate();

        if (!dbContext.Clients.Any())
        {
            foreach (var client in IdentityServerBootstrapData.Clients)
            {
                dbContext.Clients.Add(client.ToEntity());
            }
            dbContext.SaveChanges();
        }

        if (!dbContext.IdentityResources.Any())
        {
            foreach (var resource in IdentityServerBootstrapData.IdentityResources)
            {
                dbContext.IdentityResources.Add(resource.ToEntity());
            }
            dbContext.SaveChanges();
        }

        if (!dbContext.ApiScopes.Any())
        {
            foreach (var resource in IdentityServerBootstrapData.ApiScopes)
            {
                dbContext.ApiScopes.Add(resource.ToEntity());
            }
            dbContext.SaveChanges();
        }
    }
}
