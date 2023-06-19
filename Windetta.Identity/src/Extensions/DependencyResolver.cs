using Autofac;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using Windetta.Common.Authentication;
using Windetta.Common.Options;
using Windetta.Identity.Data;
using Windetta.Identity.Domain.Entities;

namespace Windetta.Identity.Extensions
{
    public static class DependencyResolver
    {
        // Adding jwt authentication
        public static void AddAuthenticationMethods(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("Authentication:Jwt").Get<JwtOptions>();

            builder.AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = jwtOptions.ValidateIssuer,
                        ValidateAudience = jwtOptions.ValidateAudience,
                        ValidateLifetime = jwtOptions.ValidateLifetime,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.ValidAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                    };
                });

            builder.AddVk(configuration);
            builder.AddGoogle(configuration);
        }

        // Add  external authentication provider 'vk.com'
        private static void AddVk(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            if (configuration["Authentication:Vk:Enabled"] == "false")
                return;

            builder.AddVkontakte("vk", options =>
            {
                options.ClientId = configuration["Authentication:Vk:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Vk:ClientSecret"]!;
            });
        }

        // Add external authentication provider 'google.com'
        private static void AddGoogle(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            if (configuration["Authentication:Google:Enabled"] == "false")
                return;

            builder.AddGoogle("google", options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
            });
        }

        // Configure Db connection to storing users, roles, claims and so on.
        public static void AddIdentityDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("Mysql").Get<MysqlSettings>();
            var connString = $"server={settings.Server};port={settings.Port};user={settings.User};password={settings.Password};database={settings.DbName}";

            services.AddDbContext<IdentityDbContext>(options => options.UseMySql(connString, new MySqlServerVersion(settings.Version),
                 b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();
        }
    }
}
