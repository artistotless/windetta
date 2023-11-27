using Autofac.Extensions.DependencyInjection;
using System.Reflection;
using Windetta.Common.Types;
using Windetta.Main.Infrastructure.SignalR;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var assembly = Assembly.GetExecutingAssembly();

//services.AddReadyMassTransit(assembly, Svc.Main);
services.AddSignalR();

services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.Authority = "http://localhost:5001"; // Àäðåñ âàøåãî ñåðâåðà IdentityServer
            options.RequireHttpsMetadata = false;
            options.Audience = "windetta.api"; // Èìÿ âàøåãî ApiResource
        });

//services.AddMysqlDbContext<WalletDbContext>(assembly);

builder.Host.UseServiceProviderFactory(
    new AutofacServiceProviderFactory(builder =>
    {
        builder.ResolveDependenciesFromAssembly();
    }));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Windetta.Main Service");
app.MapHub<MainHub>("/mainHub");
app.Run();