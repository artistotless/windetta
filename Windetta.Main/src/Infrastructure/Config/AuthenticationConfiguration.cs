using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Windetta.Common.Authentication;

namespace Windetta.Main.Infrastructure.Config;

public static class AuthenticationConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        var cfg = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        authBuilder.AddScheme<RealtimeTokenOptions, RealtimeTokenHandler>
            ("FirstConnectionAuth", options =>
            {
                var section = cfg.GetSection(nameof(RealtimeTokenOptions));

                options.PublicKey = section
                .GetValue<string>(nameof(RealtimeTokenOptions.PublicKey))
                ?? throw new Exception("Unable read RealtimeTokenOptions.PublicKey in configuration");

                options.ValidateLifetime = section
                .GetValue<bool>(nameof(RealtimeTokenOptions.ValidateLifetime));
            });

        authBuilder.AddJwtBearer(options =>
        {
            options.Authority = "https://localhost:7159";
            options.RequireHttpsMetadata = false;
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                //TODO: set TRUE
                ValidateLifetime = false,
            };
        });
    }
}
