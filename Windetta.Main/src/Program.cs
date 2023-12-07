using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Windetta.Common.Mongo;
using Windetta.Common.Types;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Infrastructure;
using Windetta.Main.Infrastructure.Data;
using Windetta.Main.Infrastructure.Security;
using Windetta.Main.Infrastructure.SignalR;

var appBuilder = WebApplication.CreateBuilder(args);
var services = appBuilder.Services;

services.AddDefaultInstanceIdProvider();
services.ConfigureAddAuthentication();
services.ConfigureAddAuthorization();
services.AddHttpContextAccessor();
services.ConfigureAddCors();
services.AddMatchHubUseCases();
services.AddMatchHubPlugins();
services.AddMongo();

services.AddSignalR(hubOptions =>
{
    hubOptions.AddFilter<SignalRExceptionFilter>();
});

services.AddSingleton<SignalRExceptionFilter>();

var autoFac = new AutofacServiceProviderFactory(builder =>
{
    builder.RegisterDecorator<HubsFromMemoryDecorator, IMatchHubs>();
    builder.ResolveDependenciesFromAssembly();
});

appBuilder.Host.UseServiceProviderFactory(autoFac);

var app = appBuilder.Build();

app.UseCors("allow_any_origins");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Windetta");
app.MapHub<MainHub>("/mainHub");

app.Run();