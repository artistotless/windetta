using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using Windetta.Common.Authentication;
using Windetta.Common.Configuration;
using Windetta.Common.Constants;
using Windetta.Main.Infrastructure.Middlewares;
using Windetta.Web.Clients;
using Windetta.Common.Host;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
builder.ConfigureComponentLaunchSettings();
builder.ConfigureClusterMap();
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery();
builder.Services.AddCors(o => o.AddPolicy(CorsPolicyNames.ALLOW_ANY, builder =>
{
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
    builder.SetIsOriginAllowed((host) => true);
    builder.AllowCredentials();
}));

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

var clusterMap = builder.Configuration
    .GetOptions<ClusterMap>(nameof(ClusterMap));

builder.Services.AddHttpClients();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "windetta.mvc";
    options.Cookie.SameSite = SameSiteMode.Lax;
})
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = clusterMap.IdentityUrl;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters();
    options.ClientId = "windetta.web";
    options.ResponseType = "code";
    options.Scope.Add("email");
    options.Scope.Add("realtime");
    options.Scope.Add("offline_access");
    options.GetClaimsFromUserInfoEndpoint = true;
    options.SaveTokens = true;
    options.ClientSecret = "secret";
    options.ClaimActions.MapUniqueJsonKey("nickname", "nickname");
    options.ClaimActions.DeleteClaim("profile");
    options.ClaimActions.DeleteClaim("family_name");
    options.Events = new OpenIdConnectEvents()
    {
        OnTicketReceived = ticket =>
        {
            if (EnvVars.FakeAuthEnabled)
                ticket.ConvertAccessTokenToFakeToken();

            return Task.CompletedTask;
        },
        OnAuthenticationFailed = c =>
        {
            c.HandleResponse();

            c.Response.StatusCode = 500;
            c.Response.ContentType = "text/plain";
            if (builder.Environment.IsDevelopment())
            {
                // Debug only, in production do not share exceptions with the remote host.
                return c.Response.WriteAsync(c.Exception.ToString());
            }
            return c.Response.WriteAsync("An error occurred processing your authentication.");
        }
    };
});

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseCors(CorsPolicyNames.ALLOW_ANY);
app.UseHttpsRedirection();
app.UseMiddleware<ProxyMiddleware>();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseOnlySingleInstanceLaunching();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
