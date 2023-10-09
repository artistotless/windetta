using Autofac.Extensions.DependencyInjection;
using System.Reflection;
using TonLibDotNet;
using Windetta.Common.Database;
using Windetta.Common.Helpers;
using Windetta.Common.RabbitMQ;
using Windetta.Common.Types;
using Windetta.Wallet.Data;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddTransient<AesEncryptor>();
services.AddSingleton<ITonClient, TonClient>();
services.AddMysqlDbContext<WalletDbContext>(Assembly.GetExecutingAssembly());

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
{
    builder.AddRabbitMq();
    builder.ResolveDependenciesFromAssembly();
}));

var app = builder.Build();

app.MapGet("/", () => "Windetta.Wallet Service");
app.MapControllers();

app.Run();