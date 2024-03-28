using Autofac.Extensions.DependencyInjection;
using Serilog;
using Windetta.Common.Configuration;
using Windetta.Common.Middlewares;
using Windetta.Common.Types;
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
        builder.ResolveDependenciesFromAssembly();
    }));

var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseCors("allow_any_origins");
app.UseAuthentication();
app.UseAuthorization();
app.UseLobbyEndpoints();
app.UseOngoingMatchesEndpoints();
app.MapGet("/", () => "Windetta");
app.MapHub<MainHub>("/mainHub");

app.Run();