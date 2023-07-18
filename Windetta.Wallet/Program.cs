using Autofac.Extensions.DependencyInjection;
using Windetta.Common.RabbitMQ;
using Windetta.Wallet.Messages.Events;
using Windetta.Common.Types;
using Windetta.Common.Constants;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
{
    builder.AddRabbitMq();
    builder.ResolveDependenciesFromAssembly();
}));

var app = builder.Build();

app.MapGet("/", () => "Windetta.Wallet Service");

app.UseRabbitMq()
    .SubscribeEvent<UserCreated>();

app.Run();
