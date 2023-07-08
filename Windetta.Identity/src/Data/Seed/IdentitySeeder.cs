using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Extensions;

namespace Windetta.Identity.Data.Seed;

public static class IdentitySeeder
{
    public static void Seed(IApplicationBuilder app)
    {
        using var userManager = app.ApplicationServices
            .GetRequiredService<UserManager<User>>();

        using var dbContext = app.ApplicationServices
           .GetRequiredService<IdentityDbContext>();
        dbContext.Database.Migrate();

        if (!userManager.Users.Any())
        {
            var rootUser = new User()
            {
                EmailConfirmed = true,
                Email = "root@root.com",
                UserName = "root",
            };

            var result = userManager.CreateAsync(rootUser, "P@ssw0rd").GetAwaiter().GetResult();

            if (!result.Succeeded)
                throw result.Errors.FirstErrorAsException();
        }
    }
}