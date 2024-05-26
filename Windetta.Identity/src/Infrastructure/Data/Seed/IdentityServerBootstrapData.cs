using IdentityServer4;
using IdentityServer4.Models;

namespace Windetta.Identity.Infrastructure.Data.Seed;

public static class IdentityServerBootstrapData
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email(),
    };

    public static IEnumerable<ApiResource> ApiResources =>
      new ApiResource[]
      {
          new ApiResource("windetta.api"),
      };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("realtime","Realtime")
            {
                Description = "Manage lobbies: create, join, leave"
            },
        };

    public static IEnumerable<Client> Clients
    {
        get
        {
            // interactive ASP.NET Core MVC client
            var client1 = new Client
            {
                Description = "Official windetta web appliccation",
                ClientId = "windetta.web",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                RequireConsent = false,
                Properties = new Dictionary<string, string>(){
               {"verified", "true"}
            },
                // where to redirect to after login
                RedirectUris = { "https://localhost:55004/signin-oidc" },
                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://localhost:55004/signout-callback-oidc" },
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

            client1.AllowedCorsOrigins.Add("https://hoppscotch.io");

            return new Client[] { client1};
        }
    }

}
