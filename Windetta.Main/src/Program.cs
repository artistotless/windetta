using Autofac.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Polly;
using System.Reflection;
using Windetta.Common.Constants;
using Windetta.Common.Database;
using Windetta.Common.MassTransit;
using Windetta.Common.Mongo;
using Windetta.Common.Types;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Matches;
using Windetta.Main.Core.Services.LSPM;
using Windetta.Main.Infrastructure;
using Windetta.Main.Infrastructure.Sagas;
using Windetta.Main.Infrastructure.Security;
using Windetta.Main.Infrastructure.SignalR;
using Windetta.Operations.Sagas;

var appBuilder = WebApplication.CreateBuilder(args);
var services = appBuilder.Services;
var assembly = Assembly.GetExecutingAssembly();

services.AddDefaultInstanceIdProvider();
services.ConfigureAddAuthentication();
services.ConfigureAddAuthorization();
services.AddHttpContextAccessor();
services.ConfigureAddCors();
services.AddMatchHub();
services.AddMatchHubPlugins();
services.AddMongo();

services.AddMysqlDbContext<SagasDbContext>(assembly);
services.AddInMemoryLspms();
services.AddReadyMassTransit(assembly, Svc.Main, cfg =>
{
    cfg.AddRequestClient<RequestGameServer>();
    cfg.SetEntityFrameworkSagaRepositoryProvider(x =>
    {
        x.ConcurrencyMode = ConcurrencyMode.Optimistic;
        x.ExistingDbContext<SagasDbContext>();
    });
});


services.AddResiliencePipeline(typeof(MatchFlow), builder =>
{
    builder.AddRetry(new()
    {
        ShouldHandle = new PredicateBuilder()
        .Handle<LspmException>(),

        MaxRetryAttempts = 10,
        Delay = TimeSpan.FromSeconds(5)
    });
});

services.AddSignalR(hubOptions =>
{
    hubOptions.AddFilter<SignalRExceptionFilter>();
});

services.AddSingleton<SignalRExceptionFilter>();

appBuilder.Host.UseServiceProviderFactory(
    new AutofacServiceProviderFactory(builder =>
    {
        builder.ResolveDependenciesFromAssembly();
    }));


var app = appBuilder.Build();

var bus = app.Services.GetService<IBus>();
await bus!.SendCommandAsync<StartSearchingGameServer>(Svc.Main, new StartSearchingGameServer()
{
    CorrelationId = Guid.NewGuid(),
    GameId = Guid.NewGuid(),
    Players = new List<Player> { new Player(Guid.NewGuid(), "Nick", 0) }
});

app.UseCors("allow_any_origins");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Windetta");
app.MapHub<MainHub>("/mainHub");

app.Run();