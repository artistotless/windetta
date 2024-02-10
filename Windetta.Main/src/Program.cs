using Autofac.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Windetta.Common.Configuration;
using Windetta.Common.Types;
using Windetta.Main.Infrastructure;
using Windetta.Main.Infrastructure.SignalR;
using Windetta.Main.Web.Api;

var builder = WebApplication.CreateBuilder(args);
IdentityModelEventSource.ShowPII = true;
builder.ConfigureComponentLaunchSettings();
builder.AddInfrastructureLayer();

builder.Host.UseServiceProviderFactory(
    new AutofacServiceProviderFactory(builder =>
    {
        builder.ResolveDependenciesFromAssembly();
    }));

var app = builder.Build();

app.UseCors("allow_any_origins");
app.UseAuthentication();
app.UseAuthorization();
app.UseLobbyEndpoints();
app.MapGet("/", () => "Windetta");
app.MapHub<MainHub>("/mainHub");

app.Run();