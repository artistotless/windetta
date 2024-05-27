using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace Windetta.Identity.Infrastructure.Data.Seed;

public static class IdentityServerConfigurationSeeder
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices
            .GetService<IServiceScopeFactory>()!.CreateScope();

        var provider = serviceScope.ServiceProvider;
        var cfg = provider.GetRequiredService<IConfiguration>();
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

        var initialClients = cfg
            .GetSection("InitialClients")
            .Get<IEnumerable<InitialClient>>();

        foreach (var item in initialClients)
        {
            // interactive ASP.NET Core MVC client
            var client = new Client
            {
                Description = item.Description,
                ClientId = item.ClientId,
                ClientSecrets = { new Secret(item.Secrets.Sha256()) },
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                RequireConsent = item.RequireConsent,
                Properties = new Dictionary<string, string>(){
               {"verified", "true"}
            },
                // where to redirect to after login
                RedirectUris = item.RedirectUris,
                // where to redirect to after logout
                PostLogoutRedirectUris = item.PostLogoutRedirectUris,
                AllowOfflineAccess = true,
                AllowedScopes = new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                IdentityServerConstants.StandardScopes.Address,
                IdentityServerConstants.StandardScopes.OfflineAccess,
                "realtime",
            }
            };

            client.AllowedCorsOrigins.Add("https://hoppscotch.io");

            dbContext.Clients.Add(client.ToEntity());
        }

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
