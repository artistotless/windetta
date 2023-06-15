using Microsoft.AspNetCore.Authentication.JwtBearer;
using Windetta.Identity.Extensions;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddIdentityDbContext(configuration);

    var authBuilder = services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    });

    // Adding vk, google .. external auth providers
    authBuilder.AddAuthenticationMethods(configuration);

    services.AddAppliacationServices();
    services.AddAuthorization();
    services.AddControllers();
}

app.MapGet("/", () => "Windetta Identity Service");

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

