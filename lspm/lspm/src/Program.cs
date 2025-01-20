using LSPM.Core.Interfaces;
using LSPM.Core.Models;
using LSPM.Core.Services;
using LSPM.Infrastructure.Consumers;
using LSPM.Infrastructure.Endpoints;
using LSPM.Infrastructure.Services;
using MassTransit;
using Polly;
using Serilog;
using Windetta.Common.Configuration;
using Windetta.Common.Constants;
using Windetta.Common.Host;
using Windetta.Common.MassTransit;
using Windetta.Contracts.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
builder.ConfigureComponentLaunchSettings();

// configure app layer
var assembly = System.Reflection.Assembly.GetExecutingAssembly();

builder.Services.AddHttpClient("match-client", (c) => { })
.AddResilienceHandler("notifier-resilience", builder =>
{
   builder.AddRetry(new Polly.Retry.RetryStrategyOptions<HttpResponseMessage>()
   {
       MaxRetryAttempts = 3,
       Delay = TimeSpan.FromSeconds(3)
   });

   builder.AddTimeout(TimeSpan.FromSeconds(10));
});

builder.Services.Configure<EndpointResolvingOptions>
    (builder.Configuration.GetSection(nameof(EndpointResolvingOptions)));
builder.Services.AddSingleton<EndpointResolver>();

builder.Services.AddReadyMassTransit(assembly, Svc.Lspm, busCfg: (cfg, context) =>
{
    var externalIp = context
    .GetRequiredService<EndpointResolver>()
    .GetExternalIp()
    .GetAwaiter()
    .GetResult();

    var formatter = context
    .GetRequiredService<IEndpointNameFormatter>();

    var endpoint = formatter.Consumer<RequestGameServerConsumer>();

    cfg.ReceiveEndpoint($"{endpoint}_{externalIp}", (x) =>
    {
        x.Durable = false;
        x.AutoDelete = true;
        x.Exclusive = true;
        x.ExclusiveConsumer = true;
        x.ConfigureConsumeTopology = false;
        x.ConfigureConsumer<RequestGameServerConsumer>(context);
        x.Bind<IGameServerRequested>(s =>
        {
            s.RoutingKey = externalIp;
            s.ExchangeType = "direct";
        });
    });
});

builder.Services.AddSingleton<IGameServersOptions, InMemoryGameServersOptions>();
builder.Services.AddSingleton<IGameServersStore, InMemoryGameServersStore>();

if (Environment.GetEnvironmentVariable("TEST_MODE")?.TrimEnd() == "Enabled")
    builder.Services.AddSingleton<IGameServerLauncher, TestModeServerLauncher>();
else
    builder.Services.AddSingleton<IGameServerLauncher, GameServerLauncher>();

builder.Services.AddSingleton<IGameServerMatchClient, GameServerMatchClient>();
builder.Services.AddScoped<GameServersFacadeService>();
builder.Services.AddScoped<ILocalServerProcessManager, LocalServerProcessManager>();
builder.Services.AddHostedService<InstanceLifetimeWatcher>();

var host = builder.Build();

host.UseWebHooks();
host.UsePublicEndpoints();
host.UseOnlySingleInstanceLaunching();
host.Run();
