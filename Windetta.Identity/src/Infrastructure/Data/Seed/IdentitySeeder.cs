using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Extensions;

namespace Windetta.Identity.Infrastructure.Data.Seed;

public static class IdentitySeeder
{
    public static void Seed(IApplicationBuilder app)
    {
        using var userManager = app.ApplicationServices
            .GetRequiredService<UserManager<User>>();

        using var roleManager = app.ApplicationServices
         .GetRequiredService<RoleManager<Role>>();

        using var dbContext =  app.ApplicationServices
           .GetRequiredService<IdentityDbContext>();

        if (!roleManager.Roles.Any())
        {
            var roles = new List<Role>()
            {
                new(Roles.USER),
                new(Roles.ADMIN),
                new(Roles.MODERATOR),
            };

            foreach (var role in roles)
                roleManager.CreateAsync(role).GetAwaiter().GetResult();
        }

        if (!userManager.Users.Any())
        {
            var rootUser = new User()
            {
                EmailConfirmed = true,
                Email = "root@root.com",
                UserName = "root",
                DisplayName = "root",
            };

            var created = userManager.CreateAsync(rootUser, "P@ssw0rd").GetAwaiter().GetResult();

            if (!created.Succeeded)
                throw created.Errors.FirstErrorAsException();

            var addedtoRole = userManager.AddToRoleAsync(rootUser, Roles.ADMIN).GetAwaiter().GetResult();

            if (!addedtoRole.Succeeded)
                throw created.Errors.FirstErrorAsException();
        }
    }
}