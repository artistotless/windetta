using MassTransit;
using System.Reflection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Operations.Data;
using Windetta.Operations.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var assembly = Assembly.GetExecutingAssembly();

services.AddSagasDbContext();
services.AddReadyMassTransit(assembly, Svc.Operations, cfg =>
{
    cfg.SetEntityFrameworkSagaRepositoryProvider(x =>
    {
        x.ConcurrencyMode = ConcurrencyMode.Optimistic;
        x.ExistingDbContext<SagasDbContext>();
    });
});

var app = builder.Build();

app.MapGet("/", () => "Windetta.Operations Service");

app.Run();