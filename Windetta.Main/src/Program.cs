using Autofac;
using Autofac.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using System.Reflection;
using Windetta.Common.Mongo;
using Windetta.Common.Types;
using Windetta.Main.Games;
using Windetta.Main.Infrastructure;
using Windetta.Main.Infrastructure.Authentication;
using Windetta.Main.Infrastructure.Authorization;
using Windetta.Main.Infrastructure.Data;
using Windetta.Main.Infrastructure.SignalR;
using Windetta.Main.MatchHub;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var assembly = Assembly.GetExecutingAssembly();

services.AddDefaultInstanceIdProvider();
services.ConfigureAddAuthentication();
services.ConfigureAddAuthorization();
services.AddMongoOptions();
services.AddSignalR();

builder.Host.UseServiceProviderFactory(
    new AutofacServiceProviderFactory(builder =>
    {
        builder.RegisterDecorator<HubsFromMemoryDecorator, IMatchHubs>();
        builder.ResolveDependenciesFromAssembly();
    }));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

app.MapGet("/", () => "Windetta");

app.MapHub<MainHub>("/mainHub");

app.Run();