using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using Windetta.Common.Configuration;
using Windetta.Common.Options;
using Windetta.Identity.Data;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Services;

namespace Windetta.Identity.Extensions
{
    public static class DependencyResolver
    {
        // Adding IdentityServer4
        public static void AddIdentityServer4(this IServiceCollection services)
        {
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

            services.AddTransient<IClaimsService, CustomClaimsService>();

            var builder = services.AddIdentityServer(options =>
            {
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            });

            using var provider = services.BuildServiceProvider();
            var settings = provider.GetRequiredService<IOptions<MysqlSettings>>().Value;
            var connectionString = GetConnectionString(settings);

            builder.AddAspNetIdentity<User>();

            builder.AddConfigurationStore(options =>
             {
                 options.ConfigureDbContext = b => b.UseMySql(connectionString, new MySqlServerVersion(settings.Version),
                     sql => sql.MigrationsAssembly(migrationsAssembly));
             });

            builder.AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseMySql(connectionString, new MySqlServerVersion(settings.Version),
                    sql => sql.MigrationsAssembly(migrationsAssembly));
            });

            builder.AddDeveloperSigningCredential();
        }

        public static void AddAuthenticationMethods(this IServiceCollection services, IConfiguration configuration)
        {
            var builder = services.AddAuthentication();

            builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
             {
                 options.Authority = "https://localhost:5001";
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateAudience = false
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
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
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
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.ClientId = configuration["Authentication:Google:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
            });
        }

        // Configure Db connection to storing users, roles, claims and so on.
        public static void AddIdentityDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MysqlSettings>(configuration.GetSection("Mysql"));

            var settings = configuration.GetOptions<MysqlSettings>("Mysql");
            var connString = GetConnectionString(settings);

            services.AddDbContext<IdentityDbContext>(options => options.UseMySql(connString, new MySqlServerVersion(settings.Version),
                 b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));

            services.AddIdentity<User, Role>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.Password.RequireDigit = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireLowercase = false;
                o.Password.RequiredLength = 6;
                o.Password.RequiredUniqueChars = 2;
            })
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();
        }

        private static string GetConnectionString(MysqlSettings settings)
            => string.Format("server={0};port={1};user={2};password={3};database={4}",
                settings.Server, settings.Port, settings.User, settings.Password, settings.DbName);
    }
}
