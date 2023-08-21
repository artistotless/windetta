using Windetta.Operations.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddSagasDbContext();
services.ConfigureMassTransit();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();