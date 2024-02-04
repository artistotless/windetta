using Autofac.Extensions.DependencyInjection;
using System.Reflection;
using Windetta.Common.Configuration;
using Windetta.Common.Constants;
using Windetta.Common.Database;
using Windetta.Common.MassTransit;
using Windetta.Common.Types;
using Windetta.Wallet.Application.DAL;
using Windetta.Wallet.Infrastructure.Data;


var builder = WebApplication.CreateBuilder(args);

builder.ConfigureComponentLaunchSettings();

var assembly = Assembly.GetExecutingAssembly();

builder.Services.AddControllers();
builder.Services.AddReadyMassTransit(assembly, Svc.Wallet);
builder.Services.AddMysqlDbContext<WalletDbContext>(assembly);
builder.Services.AddScoped<UnitOfWorkCommittable, DbUnitOfWork>();

builder.Host.UseServiceProviderFactory(
    new AutofacServiceProviderFactory(builder =>
{
    builder.ResolveDependenciesFromAssembly();
}));

var app = builder.Build();

app.MapGet("/", () => "Windetta.Wallet Service");
app.MapControllers();
app.Run();