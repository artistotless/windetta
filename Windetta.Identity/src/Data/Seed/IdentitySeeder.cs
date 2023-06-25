using Microsoft.AspNetCore.Identity;
using Windetta.Identity.Domain.Entities;

namespace Windetta.Identity.Data.Seed;

public static class IdentitySeeder
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices
            .GetService<IServiceScopeFactory>()!.CreateScope();

        var provider = serviceScope.ServiceProvider;
        var userManager = provider.GetRequiredService<UserManager<User>>();

        if (!userManager.Users.Any())
        {
            var rootUser = new User()
            {
                EmailConfirmed = true,
                Email = "root@root.com",
                UserName = "root",
                DisplayName = "root",
            };

            userManager.CreateAsync(rootUser, "root").GetAwaiter().GetResult();
        }
    }
}