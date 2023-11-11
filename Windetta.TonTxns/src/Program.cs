using Autofac.Extensions.DependencyInjection;
using System.Reflection;
using Windetta.Common.Constants;
using Windetta.Common.Database;
using Windetta.Common.MassTransit;
using Windetta.Common.Types;
using Windetta.TonTxns.Application.DAL;
using Windetta.TonTxns.Infrastructure.Data;
using Windetta.TonTxns.Infrastructure.Extensions;
using Windetta.TonTxns.Infrastructure.HostedServices;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var assembly = Assembly.GetExecutingAssembly();

services.AddReadyMassTransit(assembly, Svc.TonTxns);
services.AddMysqlDbContext<TonDbContext>(assembly);
services.AddHostedService<DepositPollerService>();
services.AddDepositAddress();
services.AddHttpTonApi();

{
    services.AddScoped<IDepositsRepository, InMemoryDepositsRepository>();
    services.AddScoped<IWithdrawalsRepository, InMemoryWithdrawalsRepository>();
    services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
}

builder.Host.UseServiceProviderFactory(
    new AutofacServiceProviderFactory(builder =>
{
    builder.ResolveDependenciesFromAssembly();
}));

var app = builder.Build();

app.MapGet("/", () => "Windetta.TonTxns Service");
app.Run();