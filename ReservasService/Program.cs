using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using ReservasService.Services;
using Shared.Data;
using Shared.EventBus;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//
// =======================
// CONFIGURATION
// =======================
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddJsonFile("appsettings.Docker.json", optional: true)
    .AddEnvironmentVariables();

//
// =======================
// Kestrel – gRPC HTTP/2
// =======================
// Usar puerto dinámico de Render ($PORT)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port), listenOptions =>
    {
        // Usar Http1AndHttp2 para compatibilidad con Render (TLS termination)
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

//
// =======================
// gRPC
// =======================
builder.Services.AddGrpc();

//
// =======================
// JWT Authentication
// =======================
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? builder.Configuration["JWT_SECRET_KEY"]
             ?? "HotelMicroservicesSecretKey2024!@#$%^&*()_+";
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
                ?? "HotelMicroservices";
var jwtAudience = builder.Configuration["Jwt:Audience"]
                  ?? "HotelMicroservicesClients";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

//
// =======================
// Repositorios
// =======================
builder.Services.AddScoped<ReservaRepository>();
builder.Services.AddScoped<HabxResRepository>();
builder.Services.AddScoped<DesxHabxResRepository>();
builder.Services.AddScoped<HoldRepository>();

//
// =======================
// EVENT BUS (RABBITMQ OPCIONAL)
// =======================
builder.Services.AddSingleton<IEventBus>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILoggerFactory>()
                   .CreateLogger("EventBus");

    var host = config["RabbitMQ:Host"];

    if (string.IsNullOrWhiteSpace(host))
    {
        logger.LogWarning("RabbitMQ no configurado, usando NullEventBus");
        return new NullEventBus();
    }

    try
    {
        return new RabbitMqEventBus(host);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "RabbitMQ no disponible, usando NullEventBus");
        return new NullEventBus();
    }
});

//
// =======================
// HealthCheck (recomendado)
// =======================
builder.Services.AddHealthChecks();

var app = builder.Build();

//
// =======================
// Middleware
// =======================
app.UseAuthentication();
app.UseAuthorization();

// Habilitar gRPC-Web para compatibilidad con proxies HTTP/1.1 (Render, Cloudflare, etc.)
app.UseGrpcWeb();

//
// =======================
// Endpoints
// =======================
app.MapGrpcService<ReservasGrpcService>().EnableGrpcWeb();

app.MapHealthChecks("/health");

app.MapGet("/", () =>
    "gRPC Reservas Service is running. Use a gRPC client.");

app.Run();
