using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Windetta.Common.Redis;
using Windetta.Common.Types;
using Windetta.Identity.Extensions;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
IServiceCollection services = builder.Services;

services.AddIdentityDbContext(configuration);
services.AddAuthorization();
services.AddControllers();
services.AddRedis(configuration);

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddAuthenticationMethods(configuration); // Adding vk, google .. external auth providers

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
{
    builder.ResolveDependenciesFromAssembly();
}));

// ---------------------

var app = builder.Build();

app.MapGet("/", () => "Windetta Identity Service");
app.MapGet("/ping", () => Results.Ok());

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
