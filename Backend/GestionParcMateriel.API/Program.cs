using GestionParcMateriel.API.Data;
using GestionParcMateriel.API.Repositories;
using GestionParcMateriel.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Entity Framework Core / SQL Server ───────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Repository Pattern ────────────────────────────────────────────────────────
builder.Services.AddScoped<IMaterielRepository, MaterielRepository>();
builder.Services.AddScoped<ITypeMaterielRepository, TypeMaterielRepository>();
builder.Services.AddScoped<IEtatMaterielRepository, EtatMaterielRepository>();
builder.Services.AddScoped<ILocalisationRepository, LocalisationRepository>();
builder.Services.AddScoped<IEmployeRepository, EmployeRepository>();
builder.Services.AddScoped<IMouvementRepository, MouvementRepository>();

// ── Services métier ───────────────────────────────────────────────────────────
builder.Services.AddScoped<IMaterielService, MaterielService>();
builder.Services.AddScoped<ITypeMaterielService, TypeMaterielService>();
builder.Services.AddScoped<IEtatMaterielService, EtatMaterielService>();
builder.Services.AddScoped<ILocalisationService, LocalisationService>();
builder.Services.AddScoped<IEmployeService, EmployeService>();
builder.Services.AddScoped<IMouvementService, MouvementService>();
builder.Services.AddHttpClient<IAiAnalysisService, AiAnalysisService>();

// ── Swagger / OpenAPI ────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gestion Parc Matériel API",
        Version = "v1",
        Description = "API de gestion du parc matériel informatique - SOPAL",
        Contact = new OpenApiContact { Name = "SOPAL IT" }
    });

    // Ajouter le support JWT dans Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez le token JWT : Bearer {votre_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"] ?? "DefaultSecretKeyForDevelopment_ChangeInProduction";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "GestionParcMateriel";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "GestionParcMaterielClient";

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueJsPolicy", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:5173",
                  "http://localhost:5174",
                  "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── Build ─────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Pipeline HTTP ─────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestion Parc Matériel v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseCors("VueJsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
