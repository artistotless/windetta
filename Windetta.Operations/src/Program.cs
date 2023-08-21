using Windetta.Operations.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var assembly = typeof(Program).Assembly;

services.AddSagasDbContext();
services.ConfigureMassTransit();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();