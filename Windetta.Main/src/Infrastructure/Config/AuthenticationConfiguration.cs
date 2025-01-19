using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Windetta.Common.Authentication;
using Windetta.Common.Configuration;
using Windetta.Common.Constants;

namespace Windetta.Main.Infrastructure.Config;

public static class AuthenticationConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = AuthSchemes.Bearer;
            options.DefaultChallengeScheme = AuthSchemes.Bearer;
        });

        if (EnvVars.FakeAuthEnabled)
        {
            authBuilder.AddScheme<FakeTokenOptions, FakeTokenHandler>(AuthSchemes.Bearer, null);
        }
        else
        {
            var clusterMap = services.BuildServiceProvider().GetRequiredService<IOptions<ClusterMap>>();

            authBuilder.AddJwtBearer(options =>
            {
                options.Authority = clusterMap.Value.IdentityUrl;
                options.RequireHttpsMetadata = false;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    //TODO: set TRUE in production
                    ValidateLifetime = false,
                };
            });
        }

        var cfg = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        authBuilder.AddScheme<RealtimeTokenOptions, RealtimeTokenHandler>
            (AuthSchemes.FirstConnectionAuth, options =>
            {
                var section = cfg.GetSection(nameof(RealtimeTokenOptions));

                options.PublicKey = section
                .GetValue<string>(nameof(RealtimeTokenOptions.PublicKey))
                ?? throw new Exception("Unable read RealtimeTokenOptions.PublicKey in configuration");

                options.ValidateLifetime = section
                .GetValue<bool>(nameof(RealtimeTokenOptions.ValidateLifetime));
            });
    }
}
