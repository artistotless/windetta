using Autofac.Extensions.DependencyInjection;
using MassTransit;
using Serilog;
using System.Reflection;
using Windetta.Common.Configuration;
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

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
builder.ConfigureComponentLaunchSettings();

var assemby = Assembly.GetExecutingAssembly();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddMysqlDbContext<IdentityDbContext>(assemby);
builder.Services.AddMysqlDbContext<SagasDbContext>(assemby);
builder.Services.AddIdentityStore();
builder.Services.AddControllersWithViews();
builder.Services.AddIdentityServer4();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRedis();
builder.Services.AddAuthenticationMethods(); // Adding vk, google .. external auth providers
builder.Services.ConfigureCustomViewsRouting();

builder.Services.AddReadyMassTransit(assemby, Svc.Identity, cfg =>
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

app.UseHsts();
app.UseStaticFiles();
app.UseIdentityServer();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", (ctx) =>
{

    return ctx.Response.WriteAsync("Windetta.Identity");

}).RequireAuthorization();

app.MapGet("/ping", () => Results.Ok());
app.Run();
