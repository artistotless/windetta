using IdentityServer4;
using IdentityServer4.Models;

namespace Windetta.Identity.Data.Seed;

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
            new ApiScope("api1")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]{
        // interactive ASP.NET Core MVC client
        new Client
        {
            ClientId = "windetta.web",
            ClientSecrets = { new Secret("secret".Sha256()) },

            AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,

            // where to redirect to after login
            RedirectUris = { "https://localhost:7129/signin-oidc" },

            // where to redirect to after logout
            PostLogoutRedirectUris = { "https://localhost:7129/signout-callback-oidc" },
            AllowOfflineAccess = true,

            AllowedScopes = new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                IdentityServerConstants.StandardScopes.Address,
                "api1"
            }
        },
    };
}
