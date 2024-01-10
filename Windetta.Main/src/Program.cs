using Autofac.Extensions.DependencyInjection;
using Windetta.Common.Types;
using Windetta.Main.Infrastructure;
using Windetta.Main.Infrastructure.SignalR;
using Windetta.Main.Web.Api;

var webHost = WebApplication.CreateBuilder(args);
var services = webHost.Services;

webHost.AddInfrastructureLayer();

webHost.Host.UseServiceProviderFactory(
    new AutofacServiceProviderFactory(builder =>
    {
        builder.ResolveDependenciesFromAssembly();
    }));

var app = webHost.Build();

app.UseCors("allow_any_origins");
app.UseAuthentication();
app.UseAuthorization();
app.UseLobbyEndpoints();
app.MapGet("/", () => "Windetta");
app.MapHub<MainHub>("/mainHub");

app.Run();