using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Windetta.Main.Infrastructure.Security;

public static class AuthenticationExtensions
{
    public static void ConfigureAddAuthentication(this IServiceCollection services)
    {
        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        authBuilder.AddJwtBearer(options =>
        {
            options.Authority = "https://localhost:7159";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessTokenFromQuery = context.Request.Query["access_token"].ToString();

                    if (!string.IsNullOrEmpty(accessTokenFromQuery))
                    {
                        // Read the token out of the query string
                        context.Token = accessTokenFromQuery;
                    }

                    return Task.CompletedTask;
                }
            };
        });
    }
}
