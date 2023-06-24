namespace Windetta.Identity.Data.Seed;

public static class IdentitySeeder
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
    }
}