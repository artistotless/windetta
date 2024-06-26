﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Windetta.Main.Infrastructure.Security;

public static class AuthenticationConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        authBuilder.AddJwtBearer(options =>
        {
            options.Authority = "https://localhost:7159";
            //options.Configuration = new OpenIdConnectConfiguration();
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                //TODO: set TRUE
                ValidateLifetime = false,
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessTokenFromQuery = context.Request.Query["access_token"].ToString();
                    var accessTokenFromHeader = context.Request.Headers.Authorization.ToString()
                    .Replace("Bearer ", string.Empty);

                    if (!string.IsNullOrEmpty(accessTokenFromQuery))
                        context.Token = accessTokenFromHeader;
                    else
                        context.Token = accessTokenFromQuery;

                    return Task.CompletedTask;
                }
            };
        });
    }
}
