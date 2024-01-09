using Autofac.Extensions.DependencyInjection;
using LSPM.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using Windetta.Common.Constants;
using Windetta.Common.Database;
using Windetta.Common.MassTransit;
using Windetta.Common.Mongo;
using Windetta.Common.Types;
using Windetta.Contracts.Events;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Infrastructure;
using Windetta.Main.Infrastructure.Logging;
using Windetta.Main.Infrastructure.Sagas;
using Windetta.Main.Infrastructure.Security;
using Windetta.Main.Infrastructure.SignalR;

var webHost = WebApplication.CreateBuilder(args);
var services = webHost.Services;
var assembly = Assembly.GetExecutingAssembly();

webHost.ConfigureAddLogging();
services.AddDefaultInstanceIdProvider();
services.ConfigureAddAuthentication();
services.ConfigureAddAuthorization();
services.AddHttpContextAccessor();
services.ConfigureAddCors();
services.AddMatchHub();
services.AddMatchHubPlugins();
services.AddMongo();
services.AddPolyRetries();
services.AddMysqlDbContext<SagasDbContext>(assembly);
services.AddInMemoryLspms();
services.AddReadyMassTransit(assembly, Svc.Main, cfg =>
{
    cfg.AddRequestClient<IGameServerRequested>();
    cfg.SetEntityFrameworkSagaRepositoryProvider(x =>
    {
        x.ConcurrencyMode = ConcurrencyMode.Optimistic;
        x.ExistingDbContext<SagasDbContext>();
    });
}, busCfg: (cfg, context) =>
{
    cfg.Send<IGameServerRequested>(x =>
    {
        x.UseRoutingKeyFormatter(context => context.Message.LspmIp);
    });

    cfg.Publish<IGameServerRequested>(x => x.ExchangeType = "direct");
});

services.AddSignalR(hubOptions =>
{
    hubOptions.AddFilter<SignalRExceptionFilter>();
});

services.AddSingleton<SignalRExceptionFilter>();

webHost.Host.UseServiceProviderFactory(
    new AutofacServiceProviderFactory(builder =>
    {
        builder.ResolveDependenciesFromAssembly();
    }));

var app = webHost.Build();

app.UseCors("allow_any_origins");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Windetta");
app.MapHub<MainHub>("/mainHub");

app.MapGet("/gameserver-request/{endpoint}", async ([FromRoute] string endpoint, IRequestClient<IGameServerRequested> client) =>
{
    var matchId = Guid.Parse("195da05a-d3ee-4d8b-917c-a77cf7afa906");
    var server = await client.GetResponse<RequestingGameServerResult>(new
    {
        CorrelationId = matchId,
        GameId = Guid.Parse("accea9d1-7f70-40e2-8a8d-a90d3a79842b"),
        Players = new[] { new Player(Guid.NewGuid(), "Nick", 0), new Player(Guid.NewGuid(), "John", 1) },
        Properties = new Dictionary<string, string>(),
        LspmKey = endpoint ?? string.Empty
    });

    return Results.Ok(server);
});

app.MapPost("/matchhub-ready", async
    (IPublishEndpoint publisher,
    MatchHubsInteractor interactor) =>
{
    var matchId = Guid.Parse("195da05a-d3ee-4d8b-917c-a77cf7afa906");
    var gameId = Guid.Parse("accea9d1-7f70-40e2-8a8d-a90d3a79842b");
    var player1Id = Guid.Parse("08dbc8b3-4170-4972-8728-c4ff931915f1");
    var player2Id = Guid.Parse("08dbc8b3-bf87-469c-8649-74c8d7b14255");

    var hub = await interactor.CreateAsync(new()
    {
        Bet = new Bet(1, 100),
        GameId = gameId,
        InitiatorId = player1Id
    });

    await interactor.JoinMemberAsync(player2Id, hub.Id, hub.Rooms.First().Index);

    await publisher.Publish<IMatchHubReady>(new
    {
        TimeStamp = DateTimeOffset.UtcNow,
        CorrelationId = hub.Id,
    });

    return Results.Ok(hub);
});

app.Run();