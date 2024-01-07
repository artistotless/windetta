using Autofac.Extensions.DependencyInjection;
using LSPM.Models;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using Windetta.Common.Constants;
using Windetta.Common.Database;
using Windetta.Common.MassTransit;
using Windetta.Common.Mongo;
using Windetta.Common.Types;
using Windetta.Contracts.Events;
using Windetta.Main.Infrastructure;
using Windetta.Main.Infrastructure.Sagas;
using Windetta.Main.Infrastructure.Security;
using Windetta.Main.Infrastructure.SignalR;

var webHost = WebApplication.CreateBuilder(args);
var services = webHost.Services;
var assembly = Assembly.GetExecutingAssembly();

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
app.MapGet("/e", async (IRequestClient<IGameServerRequested> client) =>
{
    var matchId = Guid.Parse("195da05a-d3ee-4d8b-917c-a77cf7afa906");
    var server = await client.GetResponse<RequestingGameServerResult>(new
    {
        CorrelationId = matchId,
        GameId = Guid.Parse("accea9d1-7f70-40e2-8a8d-a90d3a79842b"),
        Players = new[] { new Player(Guid.NewGuid(), "Nick", 0), new Player(Guid.NewGuid(), "John", 1) },
        Properties = new Dictionary<string, string>(),
        LspmKey = string.Empty
    });

    return Results.Ok(server);
});

app.Run();