using System.Security.Cryptography;
using Afrisan.Nuevo.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Primera entrega de revisión: no hay conexión ni escrituras a PostgreSQL/Supabase.
// La producción deberá registrar un proveedor persistente y autenticación real.
if (!builder.Environment.IsDevelopment())
    throw new InvalidOperationException("Esta versión de revisión solo funciona en Development y guarda datos en memoria.");

var firma = RandomNumberGenerator.GetBytes(64);
builder.Services.AddSingleton(new SesionesDemo(firma));
builder.Services.AddSingleton<InventarioStore>();
builder.Services.AddSingleton<TrabajoStore>();
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = 6 * 1024 * 1024);
builder.Services.AddProblemDetails();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidIssuer = SesionesDemo.Issuer,
        ValidateAudience = true, ValidAudience = SesionesDemo.Audience,
        ValidateLifetime = true, ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(firma)
    };
});
builder.Services.AddAuthorization();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins("http://localhost:5286").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "AFRISAN Nuevo · Inventario en memoria", Version = "v1" });
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT" });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = Array.Empty<string>()
    });
});
var app = builder.Build();
app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapInventario();
app.MapTrabajo();
app.Logger.LogInformation("AFRISAN NUEVO: inventario vacío en memoria; no conectado a Supabase. Reiniciar borra los registros.");
app.Run();
