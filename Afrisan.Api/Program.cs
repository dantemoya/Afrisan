using Afrisan.Api.Data;
using Afrisan.Api.Models;
using Afrisan.Api.Demo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;

var initializeDatabase = args.Contains("--initialize-database", StringComparer.Ordinal);
var builder = WebApplication.CreateBuilder(
    args.Where(arg => arg != "--initialize-database").ToArray());

var demo = builder.Configuration.GetValue<bool>("Demo:Enabled");
if (demo && !builder.Environment.IsDevelopment())
    throw new InvalidOperationException("El modo demo solo puede iniciarse en Development.");
if (demo && initializeDatabase)
    throw new InvalidOperationException("Desactiva Demo:Enabled para inicializar una base real.");

// =====================================================
// KESTREL
// =====================================================

// Los perfiles http/https/RedLocal controlan los puertos. No se fuerza HTTP
// sobre el perfil HTTPS, para que la dirección que usa la Web sea coherente.

// =====================================================
// BASE DE DATOS
// =====================================================

if (demo)
{
    builder.Services.AddSingleton<DemoStore>();
    builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
    builder.Services.Configure<Microsoft.AspNetCore.DataProtection.KeyManagement.KeyManagementOptions>(
        options => options.XmlRepository = new DemoKeyRepository());
    // Credenciales de firma efímeras y emisor propios: no se reutilizan secretos
    // reales ni se permite que estos tokens entren en el entorno con base de datos.
    builder.Configuration["Jwt:Issuer"] = "AFRISAN.Demo";
    builder.Configuration["Jwt:Audience"] = "AFRISAN.Demo.Web";
    builder.Configuration["Jwt:Key"] = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
else
{
    var connectionString =
        builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "No se encontró la cadena de conexión 'DefaultConnection'."
        );
    }

    builder.Services.AddDbContext<AfrisanDbContext>(options =>
        options.UseNpgsql(connectionString)
    );

    // =====================================================
    // IDENTITY
    // =====================================================

    builder.Services
        .AddIdentity<Usuario, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;

            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan =
                TimeSpan.FromMinutes(15);
        })
        .AddEntityFrameworkStores<AfrisanDbContext>()
        .AddUserStore<UsuarioStore>()
        .AddDefaultTokenProviders();

    // Comando explícito: crea/aplica el esquema y los roles sin arrancar el servidor HTTP.
    if (initializeDatabase)
    {
        await using var initializationApp = builder.Build();
        await DatabaseInitializer.InitializeAsync(
            initializationApp.Services, initializationApp.Configuration);
        return;
    }
}

// =====================================================
// JWT
// =====================================================

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtIssuer) ||
    string.IsNullOrWhiteSpace(jwtAudience) ||
    string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "Falta configurar Jwt:Issuer, Jwt:Audience o Jwt:Key."
    );
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    ),

                ClockSkew = TimeSpan.Zero
            };
    });

// =====================================================
// CORS
// =====================================================

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AfrisanWeb", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            if (!Uri.TryCreate(
                origin,
                UriKind.Absolute,
                out var uri))
            {
                return false;
            }

            bool puertoPermitido =
                (uri.Scheme == "http" && uri.Port == 5226) ||
                (uri.Scheme == "https" && uri.Port == 7083);

            if (!puertoPermitido)
            {
                return false;
            }

            if (uri.Host.Equals(
                "localhost",
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (!IPAddress.TryParse(uri.Host, out var ip))
            {
                return false;
            }

            if (IPAddress.IsLoopback(ip))
            {
                return true;
            }

            if (ip.AddressFamily != AddressFamily.InterNetwork)
            {
                return false;
            }

            byte[] bytes = ip.GetAddressBytes();

            return
                bytes[0] == 10 ||
                (bytes[0] == 172 &&
                 bytes[1] >= 16 &&
                 bytes[1] <= 31) ||
                (bytes[0] == 192 &&
                 bytes[1] == 168);
        })
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// =====================================================
// CONTROLADORES
// =====================================================

if (!demo) builder.Services.AddControllers();

if (!demo) builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database");

// =====================================================
// SWAGGER + JWT
// =====================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Ingresa el token JWT."
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});

// =====================================================
// APP
// =====================================================

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseCors("AfrisanWeb");

// Siempre autenticación antes de autorización.
app.UseAuthentication();
app.UseAuthorization();

if (demo)
{
    app.MapDemoEndpoints();
    app.Logger.LogInformation("AFRISAN DEMO: datos simulados en memoria. Se reinician al cerrar la API.");
}
else
{
    app.MapControllers();
    app.MapHealthChecks("/health/ready");
    app.MapGet("/api/entorno", () => Results.Ok(new { demo = false })).AllowAnonymous();
}

// =====================================================
// SEED DE ROLES Y ADMINISTRADOR
// =====================================================

if (!demo)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AfrisanDbContext>();
    if ((await db.Database.GetPendingMigrationsAsync()).Any())
        throw new InvalidOperationException(
            "Hay migraciones pendientes. Revisa Afrisan.Api/Database/README.md antes de iniciar la API.");
    await IdentitySeeder.InicializarAsync(
        scope.ServiceProvider,
        app.Configuration);
}

app.Run();
