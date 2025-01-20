using Autofac.Extensions.DependencyInjection;
using Serilog;
using System.Reflection;
using Windetta.Common.Configuration;
using Windetta.Common.Constants;
using Windetta.Common.Database;
using Windetta.Common.Host;
using Windetta.Common.MassTransit;
using Windetta.Common.Types;
using Windetta.Contracts.Events;
using Windetta.Wallet.Application.DAL;
using Windetta.Wallet.Infrastructure.Data;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
builder.ConfigureComponentLaunchSettings();

var assembly = Assembly.GetExecutingAssembly();

builder.Services.AddControllers();
builder.Services.AddReadyMassTransit(assembly, Svc.Wallet, regCfg =>
{
    regCfg.AddRequestClient<IBalanceRequested>();
});

builder.Services.AddMysqlDbContext<WalletDbContext>(assembly, applyMigrations: true);
builder.Services.AddScoped<UnitOfWork, DbUnitOfWork>();

builder.Host.UseServiceProviderFactory(
    new AutofacServiceProviderFactory(builder =>
{
    builder.ResolveDependenciesFromAssembly();
}));

var app = builder.Build();

app.MapGet("/", () => "Windetta.Wallet Service");
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseOnlySingleInstanceLaunching();
app.Run();