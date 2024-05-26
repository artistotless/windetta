using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using System.Reflection;
using Windetta.Common.Configuration;
using Windetta.Common.Middlewares;
using Windetta.Common.Types;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Infrastructure;
using Windetta.Main.Infrastructure.SignalR;
using Windetta.Main.Web.Api;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
builder.ConfigureComponentLaunchSettings();
builder.AddInfrastructureLayer();

builder.Host.UseServiceProviderFactory(
    new AutofacServiceProviderFactory(builder =>
    {
        builder.ResolveDependenciesFromAssembly(Assembly.GetAssembly(typeof(ILobby)));
        builder.ResolveDependenciesFromAssembly();
    }));

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseCors("allow_any_origins");
app.UseAuthentication();
app.UseAuthorization();
app.UseLobbyEndpoints();
app.UseOngoingMatchesEndpoints();
app.UseGameUIsEndpoints();
app.MapGet("/", () => "Windetta");
app.MapHub<MainHub>("/mainHub");

app.Run();