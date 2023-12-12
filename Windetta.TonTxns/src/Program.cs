using Autofac.Extensions.DependencyInjection;
using MassTransit;
using System.Reflection;
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
var services = builder.Services;
var configuration = builder.Configuration;
var assembly = Assembly.GetExecutingAssembly();

services.AddMysqlDbContext<TonDbContext>(assembly);
services.AddMysqlDbContext<SagasDbContext>(assembly);
services.AddHostedService<DepositPollerService>();
services.AddDepositAddress();
services.AddHttpTonApi();

{
    services.AddScoped<IDepositsRepository, InMemoryDepositsRepository>();
    services.AddScoped<IWithdrawalsRepository, InMemoryWithdrawalsRepository>();
    services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
}

services.AddReadyMassTransit(assembly, Svc.TonTxns, cfg =>
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