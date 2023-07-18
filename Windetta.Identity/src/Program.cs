using Autofac.Extensions.DependencyInjection;
using Windetta.Common.RabbitMQ;
using Windetta.Common.Redis;
using Windetta.Common.Types;
using Windetta.Identity.Data.Seed;
using Windetta.Identity.Extensions;
using Windetta.Identity.Mvc;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
IServiceCollection services = builder.Services;

services.AddIdentityDbContext();
services.AddControllersWithViews();
services.AddIdentityServer4();
services.AddAuthorization();
services.AddControllers();
services.AddHttpContextAccessor();
services.AddRedis();
services.AddAuthenticationMethods(); // Adding vk, google .. external auth providers
services.ConfigureCustomViewsRouting();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
{
    builder.AddRabbitMq();
    builder.ResolveDependenciesFromAssembly();
}));

// ---------------------

var app = builder.Build();

IdentitySeeder.Seed(app);
IdentityServerConfigurationSeeder.Seed(app);

app.MapGet("/", () => "Windetta Identity Service").RequireAuthorization();
app.MapGet("/ping", () => Results.Ok());


app.UseStaticFiles();
app.UseIdentityServer();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
