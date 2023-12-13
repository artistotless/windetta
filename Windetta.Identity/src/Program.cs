using Autofac.Extensions.DependencyInjection;
using MassTransit;
using System.Reflection;
using Windetta.Common.Constants;
using Windetta.Common.Database;
using Windetta.Common.MassTransit;
using Windetta.Common.Redis;
using Windetta.Common.Types;
using Windetta.Identity.Extensions;
using Windetta.Identity.Infrastructure.Data;
using Windetta.Identity.Infrastructure.Data.Seed;
using Windetta.Identity.Infrastructure.Sagas;
using Windetta.Identity.Mvc;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
IServiceCollection services = builder.Services;
var assemby = Assembly.GetExecutingAssembly();

services.AddReadyMassTransit(assemby, Svc.Identity);
services.AddMysqlDbContext<IdentityDbContext>(assemby);
services.AddMysqlDbContext<SagasDbContext>(assemby);
services.AddIdentityStore();
services.AddControllersWithViews();
services.AddIdentityServer4();
services.AddAuthorization();
services.AddControllers();
services.AddHttpContextAccessor();
services.AddRedis();
services.AddAuthenticationMethods(); // Adding vk, google .. external auth providers
services.ConfigureCustomViewsRouting();

services.AddReadyMassTransit(assemby, Svc.Identity, cfg =>
{
    cfg.SetEntityFrameworkSagaRepositoryProvider(x =>
    {
        x.ConcurrencyMode = ConcurrencyMode.Optimistic;
        x.ExistingDbContext<SagasDbContext>();
    });
});

builder.Host.UseServiceProviderFactory(
    new AutofacServiceProviderFactory(builder =>
{
    builder.ResolveDependenciesFromAssembly();
}));

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
