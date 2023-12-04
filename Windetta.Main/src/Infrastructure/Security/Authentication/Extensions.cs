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
                    var accessTokenFromHeader = context.Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", string.Empty);

                    var access_token = string.IsNullOrEmpty(accessTokenFromQuery) ? accessTokenFromHeader : accessTokenFromQuery;

                    if (!string.IsNullOrEmpty(access_token))
                    {
                        // Read the token out of the query string
                        context.Token = access_token;
                    }
                    return Task.CompletedTask;
                }
            };
        });
    }
}
