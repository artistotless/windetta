using Autofac.Extensions.DependencyInjection;
using MassTransit;
using Serilog;
using System.Reflection;
using Windetta.Common.Configuration;
using Windetta.Common.Constants;
using Windetta.Common.Database;
using Windetta.Common.MassTransit;
using Windetta.Common.Types;
using Windetta.TonTxns.Application.DAL;
using Windetta.TonTxns.Infrastructure.Data;
using Windetta.TonTxns.Infrastructure.Extensions;
using Windetta.TonTxns.Infrastructure.HostedServices;
using Windetta.TonTxns.Infrastructure.Sagas;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
builder.ConfigureComponentLaunchSettings();

var assembly = Assembly.GetExecutingAssembly();

builder.Services.AddMysqlDbContext<TonDbContext>(assembly, applyMigrations: true);
builder.Services.AddMysqlDbContext<SagasDbContext>(assembly, applyMigrations: true);
builder.Services.AddHostedService<DepositPollerService>();
builder.Services.AddDepositAddress();
builder.Services.AddHttpTonApi();

{
    builder.Services.AddScoped<IDepositsRepository, InMemoryDepositsRepository>();
    builder.Services.AddScoped<IWithdrawalsRepository, InMemoryWithdrawalsRepository>();
    builder.Services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
}

builder.Services.AddReadyMassTransit(assembly, Svc.TonTxns, cfg =>
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

app.MapGet("/", () => "Windetta.TonTxns Service");
app.Run();