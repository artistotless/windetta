using Autofac.Extensions.DependencyInjection;
using System.Reflection;
using Windetta.Common.Constants;
using Windetta.Common.Database;
using Windetta.Common.MassTransit;
using Windetta.Common.RabbitMQ;
using Windetta.Common.Types;
using Windetta.TonTxns.Data;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var assembly = Assembly.GetExecutingAssembly();

services.AddMassTransit(assembly, Svc.TonTxns);
services.AddMysqlDbContext<TonDbContext>(assembly);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
{
    builder.AddRabbitMq();
    builder.ResolveDependenciesFromAssembly();
}));

var app = builder.Build();

app.MapGet("/", () => "Windetta.TonTxns Service");
app.Run();