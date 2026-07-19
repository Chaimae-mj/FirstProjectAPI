using FirstProjectAPI.Infra;
using FirstProjectAPI.Services;
using FirstProjectAPI.Services.Ai;
using FirstProjectAPI.Services.Avatar;
using FirstProjectAPI.Services.Dashboard;
using FirstProjectAPI.Services.Formateur;
using FirstProjectAPI.Services.Session;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.Text;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IFormateurService, FormateurService>();
builder.Services.AddScoped<IReponseApprenantService, ReponseApprenantService>();
builder.Services.AddScoped<IModaliteService, ModaliteService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddHttpClient<IAiService, AiService>();
builder.Services.AddSingleton<AvatarSessionManager>();
builder.Services.AddHttpClient<IAvatarService, AvatarService>();
// 1. Gestion des contrôleurs et blocage des boucles infinies JSON (très important pour les API)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

// 2. Configuration de TON DbContext (MyContext) avec MySQL / Pomelo
builder.Services.AddDbContext<MyContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// 3. Injection de tes dépendances (DAO et Services)
builder.Services.AddScoped<IDao, DaoImpl>();
builder.Services.AddScoped<IServices, VService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 4. Authentification JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"]
    ?? throw new InvalidOperationException("Jwt:Key manquant dans appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSection["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddAuthorization();

// CORS — dev uniquement, pour permettre à la page de test HTML/LiveKit
// (ouverte en file:// ou via un petit serveur local) d'appeler l'API.
// À restreindre à des origines précises avant un déploiement en production.
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevTestPage", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 5. Swagger avec support du bouton "Authorize" (Bearer token)
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Entrer uniquement le token JWT (sans le préfixe 'Bearer ')."
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configuration du pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("DevTestPage");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();