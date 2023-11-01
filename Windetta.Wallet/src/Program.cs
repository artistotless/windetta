using Autofac.Extensions.DependencyInjection;
using System.Reflection;
using Windetta.Common.Constants;
using Windetta.Common.Database;
using Windetta.Common.Helpers;
using Windetta.Common.MassTransit;
using Windetta.Common.RabbitMQ;
using Windetta.Common.Types;
using Windetta.Wallet.Data;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var assembly = Assembly.GetExecutingAssembly();

services.AddControllers();
services.AddMassTransit(assembly, Svc.Wallet);
services.AddTransient<AesEncryptor>();
services.AddMysqlDbContext<WalletDbContext>(assembly);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
{
    builder.AddRabbitMq();
    builder.ResolveDependenciesFromAssembly();
}));

var app = builder.Build();

app.MapGet("/", () => "Windetta.Wallet Service");
app.MapControllers();
app.Run();