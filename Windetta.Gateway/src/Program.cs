var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Windetta API Gateway");

//services.AddAuthentication("Bearer")
//        .AddJwtBearer("Bearer", options =>
//        {
//            options.Authority = "http://localhost:5001"; // ����� ������ ������� IdentityServer
//            options.RequireHttpsMetadata = false;

//            options.Audience = "windetta.api"; // ��� ������ ApiResource
//        });

app.Run();
