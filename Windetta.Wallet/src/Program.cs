using Autofac.Extensions.DependencyInjection;
using System.Reflection;
using Windetta.Common.Constants;
using Windetta.Common.Database;
using Windetta.Common.Helpers;
using Windetta.Common.MassTransit;
using Windetta.Common.RabbitMQ;
using Windetta.Common.Types;
using Windetta.Wallet.Data;
using Windetta.Wallet.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var assembly = Assembly.GetExecutingAssembly();

services.AddControllers();
services.AddMassTransit(assembly, Svc.Wallet);
services.AddTransient<AesEncryptor>();
services.AddHttpTonApi();
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