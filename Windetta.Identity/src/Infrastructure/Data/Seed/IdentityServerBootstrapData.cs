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
                RequireConsent = true,
                Properties = new Dictionary<string, string>(){
               {"verified", "true"}
            },
                // where to redirect to after login
                RedirectUris = { "https://localhost:7084/signin-oidc" },
                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://localhost:7084/signout-callback-oidc" },
                AllowOfflineAccess = true,
                AllowedScopes = new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                IdentityServerConstants.StandardScopes.Address,
                "realtime",
            }
            };

            //JavaScript Client
            var client2 = new Client
            {
                Description = "JavaScript Client",
                ClientId = "js",
                ClientName = "JavaScript Client",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RedirectUris = { "https://localhost:5003/callback.html" },
                PostLogoutRedirectUris = { "https://localhost:5003/index.html" },
                AllowedCorsOrigins = { "https://localhost:5003" },
                RequireConsent = true,

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "realtime"
                }
            };

            client1.AllowedCorsOrigins.Add("https://hoppscotch.io");

            return new Client[] { client1, client2 };
        }
    }

}
