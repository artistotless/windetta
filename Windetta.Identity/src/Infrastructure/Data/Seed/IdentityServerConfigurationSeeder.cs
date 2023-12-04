using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Windetta.Identity.Infrastructure.Data.Seed;

public static class IdentityServerConfigurationSeeder
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices
            .GetService<IServiceScopeFactory>()!.CreateScope();

        var provider = serviceScope.ServiceProvider;
        var dbContext = provider.GetRequiredService<ConfigurationDbContext>();
        var persisteddbContext = provider.GetRequiredService<PersistedGrantDbContext>();

        dbContext.Database.Migrate();
        persisteddbContext.Database.Migrate();

        #region IdentityResources
        var identityResources = dbContext.IdentityResources.ToList();

        if (identityResources.Count > 0)
        {
            dbContext.IdentityResources.RemoveRange(identityResources);
            dbContext.SaveChanges();
        }

        foreach (var resource in IdentityServerBootstrapData.IdentityResources)
            dbContext.IdentityResources.Add(resource.ToEntity());

        dbContext.SaveChanges();
        #endregion

        #region Clients
        var clients = dbContext.Clients.ToList();

        if (clients.Count > 0)
        {
            dbContext.Clients.RemoveRange(clients);
            dbContext.SaveChanges();
        }

        foreach (var client in IdentityServerBootstrapData.Clients)
            dbContext.Clients.Add(client.ToEntity());

        dbContext.SaveChanges();
        #endregion

        #region ApiScopes
        var apiScopes = dbContext.ApiScopes.ToList();

        if (apiScopes.Count > 0)
        {
            dbContext.ApiScopes.RemoveRange(apiScopes);
            dbContext.SaveChanges();
        }

        foreach (var resource in IdentityServerBootstrapData.ApiScopes)
            dbContext.ApiScopes.Add(resource.ToEntity());

        dbContext.SaveChanges();
        #endregion

        #region ApiResources
        var apiResources = dbContext.ApiResources.ToList();

        if (apiResources.Count > 0)
        {
            dbContext.ApiResources.RemoveRange(apiResources);
            dbContext.SaveChanges();
        }

        foreach (var resource in IdentityServerBootstrapData.ApiResources)
            dbContext.ApiResources.Add(resource.ToEntity());

        dbContext.SaveChanges();
        #endregion
    }
}
