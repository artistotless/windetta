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
}
